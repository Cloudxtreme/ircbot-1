using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Web;

using Meebey.SmartIrc4net;

using IrcBot.Client.Triggers.Contracts;

namespace IrcBot.Client.Triggers
{
    public class GoogleImageSearchTrigger : IGoogleImageSearchTrigger
    {
        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length <= 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !gimage <search>");
                return;
            }

            var query = HttpUtility.UrlDecode(string.Join(" ", triggerArgs));
            var request = WebRequest.Create($"https://ajax.googleapis.com/ajax/services/search/images?v=1.0&q={query}") as HttpWebRequest;

            if (request == null)
            {
                return;
            }

            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response == null || response.StatusCode != HttpStatusCode.OK)
                {
                    return;
                }

                var jsonSerializer = new DataContractJsonSerializer(typeof(GoogleImageSearchResponse));
                var objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                var jsonResponse = objResponse as GoogleImageSearchResponse;

                if (jsonResponse == null || jsonResponse.ResponseStatus != 200 || jsonResponse.Data == null || jsonResponse.Data.Results.Length <= 0)
                {
                    return;
                }

                foreach (var item in jsonResponse.Data.Results)
                {
                    client.SendMessage(SendType.Message, eventArgs.Data.Channel,
                        $"{item.Content} - {item.Url}");
                }
            }
        }

        [DataContract]
        private class GoogleImageSearchResponse
        {
            [DataMember(Name = "responseData")]
            public ResponseData Data { get; set; }

            [DataMember(Name = "responseStatus")]
            public int ResponseStatus { get; set; }
        }

        [DataContract]
        private class ResponseData
        {
            [DataMember(Name = "results")]
            public SearchResult[] Results { get; set; }
        }

        [DataContract]
        private class SearchResult
        {
            [DataMember(Name = "url")]
            public string Url { get; set; }

            [DataMember(Name = "contentNoFormatting")]
            public string Content { get; set; }
        }
    }
}
