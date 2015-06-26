using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

using IrcBot.Entities.Dto;
using IrcBot.Entities.Models;
using IrcBot.Service;

namespace IrcBot.Web.Controllers.Api
{
    public class MessagesController : ApiController
    {
        private readonly IMessageService _messageService;

        public MessagesController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        [Route("api/messages")]
        public async Task<HttpResponseMessage> Get()
        {
            var messages = await _messageService
                .Query()
                .SelectAsync();

            var enumerable = messages as Message[] ?? messages.ToArray();
            var result = new MessageDto[enumerable.Length];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = new MessageDto(enumerable[i]);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/messages")]
        public async Task<HttpResponseMessage> Get(DateTime from, DateTime to)
        {
            var messages = await _messageService
                .Query(x => x.Created >= from && x.Created <= to)
                .SelectAsync();

            var enumerable = messages as Message[] ?? messages.ToArray();
            var result = new MessageDto[enumerable.Length];

            for (var i = 0; i < result.Length; i++)
            {
                result[i] = new MessageDto(enumerable[i]);
            }

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        [Route("api/messages/{id:int}")]
        public async Task<HttpResponseMessage> Get(int id)
        {
            var message = await _messageService.FindAsync(id);

            return Request.CreateResponse(HttpStatusCode.OK, new MessageDto(message));
        }
    }
}
