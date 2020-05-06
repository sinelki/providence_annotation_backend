using System;
using System.Collections.Generic;
using Childes.DataContracts;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;
using Newtonsoft.Json;

namespace Childes.ServiceContracts
{
    [JsonObject]
    public class NounPhraseContext : IPhraseContext
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("focusBegin")]
        public int FocusBegin { get; set; }

        [JsonProperty("focusEnd")]
        public int FocusEnd { get; set; }

        [JsonProperty("context")]
        public IEnumerable<string> Context { get; set; }
    }
}
