using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSourcing.ServiceContracts;
using Newtonsoft.Json;

namespace Childes.ServiceContracts
{
    [JsonObject]
    public class NounPhrase : IPhraseConstruct
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("phrase")]
        public string Phrase { get; set; }
    }
}
