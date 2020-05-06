using CrowdSourcing.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Childes.DataContracts
{
    public class Video : IMedia
    {
        public Video(string content)
        {
            this.Content = content;
        }

        public string Content { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        public byte[] Bytes()
        {
            return Convert.FromBase64String(this.Content);
        }

    }
}
