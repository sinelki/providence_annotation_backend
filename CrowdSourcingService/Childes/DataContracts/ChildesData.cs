using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSourcing.DataContracts;

namespace Childes.DataContracts
{
    public abstract class ChildesData
    {
        public abstract void SetProperty(string property, object value);
    }
}
