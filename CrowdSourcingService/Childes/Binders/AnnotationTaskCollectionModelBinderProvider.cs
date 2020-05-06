using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrowdSourcing.ServiceContracts;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Childes.Binders
{
    public class AnnotationTaskCollectionModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Metadata.ModelType == typeof(IEnumerable<IAnnotationTask>))
            {
                return new AnnotationTaskCollectionModelBinder();
            }

            return null;
        }
    }
}
