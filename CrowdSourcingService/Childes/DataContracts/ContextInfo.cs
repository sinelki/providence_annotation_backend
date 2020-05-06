using System;
using System.Collections.Generic;
using CrowdSourcing.DataContracts;
using Newtonsoft.Json;

namespace Childes.DataContracts
{
    [JsonObject(IsReference=true)]
    public class ContextInfo : ChildesData, IContextInfo
    {
        public ContextInfo()
        {
            this.Properties = new Dictionary<string, object>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("correlationId")]
        public string CorrelationId { get; set; }

        [JsonProperty("contextIds")]
        public IEnumerable<string> TranscriptIds { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Properties { get; set; }

        public override void SetProperty(string propertyKey, object val)
        {
            if (propertyKey.Equals("target_utterance", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Id = val.ToString();
                this.CorrelationId = this.Id;
            }
            else if (propertyKey.Equals("context_utterances", StringComparison.InvariantCultureIgnoreCase))
            {
                this.TranscriptIds = val.ToString().Split(",");
                //this.TranscriptIds = JsonConvert.DeserializeObject<List<string>>(val.ToString());
            }
            else
            {
                this.Properties[propertyKey] = val;
            }
        }
    }
}
