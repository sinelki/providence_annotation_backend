using System;
using System.Collections.Generic;
using CrowdSourcing.DataContracts;
using Newtonsoft.Json;

namespace Childes.DataContracts
{
    [JsonObject("utterance", IsReference = true)]
    public class Utterance : ChildesData, ITranscriptElement
    {
        public Utterance()
        {
            this.Properties = new Dictionary<string, object>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("phrase")]
        public string Phrase { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Properties { get; set; }

        public override void SetProperty(string propertyKey, object val)
        {
            if (propertyKey.Equals("speaker_code", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Author = (string)val;
            }
            else if (propertyKey.Equals("gloss", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Phrase = (string)val;
            }
            else if (propertyKey.Equals("target_utterance", StringComparison.InvariantCultureIgnoreCase)
                || propertyKey.Equals("id", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Id = val.ToString();
            }
            else
            {
                this.Properties[propertyKey] = val;
            }
        }
    }
}
