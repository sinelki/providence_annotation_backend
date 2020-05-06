using System;
using System.Collections.Generic;
using CrowdSourcing.DataContracts;
using Newtonsoft.Json;

namespace Childes.DataContracts
{
    [JsonObject("bucket", IsReference = true)]
    public class UtteranceBucketInfo : ChildesData, IBucketInfo
    {
        public UtteranceBucketInfo()
        {
            this.Id = -1;
            this.Begin = -1;
            this.End = -1;

            this.Properties = new Dictionary<string, object>();
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("start_row")]
        public int Begin { get; set; }

        [JsonProperty("end_row")]
        public int End { get; set; }

        [JsonExtensionData]
        public IDictionary<string, object> Properties { get; set; }

        public bool IsValid()
        {
            if (this.Id < 0 || this.Begin < 0 || this.End < 0)
            {
                return false;
            }

            return true;
        }

        public override void SetProperty(string propertyKey, object val)
        {
            if (propertyKey.Equals("id", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Id = (int)val;
            }
            else if (propertyKey.Equals("start_row", StringComparison.InvariantCultureIgnoreCase))
            {
                this.Begin = (int)val;
            }
            else if (propertyKey.Equals("end_row", StringComparison.InvariantCultureIgnoreCase))
            {
                this.End = (int)val; ;
            }
            else
            {
                this.Properties[propertyKey] = val;
            }
        }
    }
}
