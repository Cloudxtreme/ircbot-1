using System;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

using Meebey.SmartIrc4net;

namespace IrcBot.Client.Triggers
{
    public class UrbanDictionaryTrigger : ITrigger
    {
        public void Execute(IrcClient client, IrcEventArgs eventArgs, string[] triggerArgs)
        {
            if (triggerArgs.Length == 0)
            {
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, "Syntax: !ud <search>");
                return;
            }

            var query = String.Join(" ", triggerArgs);
            var request = WebRequest.Create(String.Format("http://api.urbandictionary.com/v0/define?term={0}", query)) as HttpWebRequest;

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

                var jsonSerializer = new DataContractJsonSerializer(typeof(UrbanDictionaryResponse));
                var objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                var jsonResponse = objResponse as UrbanDictionaryResponse;

                if (jsonResponse == null || jsonResponse.Definitions.Length <= 0)
                {
                    return;
                }

                client.SendMessage(SendType.Message, eventArgs.Data.Channel, jsonResponse.Definitions[0].Definition.Replace("\r\n", ""));
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, jsonResponse.Definitions[0].Example.Replace("\r\n", ""));
                client.SendMessage(SendType.Message, eventArgs.Data.Channel, jsonResponse.Definitions[0].Permalink.Replace("\r\n", ""));
            }
        }

        [DataContract]
        private class UrbanDictionaryResponse
        {
            [DataMember(Name = "tags")]
            public string[] Tags { get; set; }

            [DataMember(Name = "result_type")]
            public string ResultType { get; set; }

            [DataMember(Name = "list")]
            public UrbanDictionaryDefinition[] Definitions { get; set; }
        }

        [DataContract]
        private class UrbanDictionaryDefinition
        {
            [DataMember(Name = "word")]
            public string Word { get; set; }

            [DataMember(Name = "author")]
            public string Author { get; set; }

            [DataMember(Name = "permalink")]
            public string Permalink { get; set; }

            [DataMember(Name = "definition")]
            public string Definition { get; set; }

            [DataMember(Name = "example")]
            public string Example { get; set; }
        }
    }
}
