using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSourcing.DataContracts;

namespace Childes.Extensions
{
    public static class PropertyBagValidator
    {
        static readonly string[] RequiredAnnotationKeys = { };

        public static bool ValidateAsAnnotation(this IPropertyBag propertyBag)
        {
            if (propertyBag == null)
            {
                return false;
            }

            foreach (string requiredKey in RequiredAnnotationKeys)
            {
                if (!propertyBag.Properties.Keys.Contains(requiredKey))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
