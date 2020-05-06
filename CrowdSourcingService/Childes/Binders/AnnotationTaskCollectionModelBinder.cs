using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Childes.ServiceContracts;
using CrowdSourcing.ServiceContracts;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Childes.Binders
{
    public class AnnotationTaskCollectionModelBinder : IModelBinder
    {
        readonly JsonSerializer serializer = new JsonSerializer();
        readonly JsonSerializerSettings serializationSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto
        };

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            if (bindingContext.HttpContext.Request.Body.CanRead)
            {
                List<AnnotationTask> tasks = new List<AnnotationTask>();
                using (StreamReader reader = new StreamReader(bindingContext.HttpContext.Request.Body))
                {
                    using (JsonReader jsonReader = new JsonTextReader(reader))
                    {
                        while (jsonReader.Read())
                        {
                            if (jsonReader.TokenType == JsonToken.StartObject)
                            {
                                AnnotationTask task = serializer.Deserialize<AnnotationTask>(jsonReader);
                                tasks.Add(task);
                            }
                        }
                    }
                }

                bindingContext.Result = ModelBindingResult.Success(tasks);
            }

            return Task.CompletedTask;
        }
    }
}
