using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CrowdSourcing.Core;
using CrowdSourcing.ServiceContracts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private IAnnotationService annotationService;
        private IMediaStreamer mediaStreamer;

        public TasksController(IEnumerable<IAnnotationService> annotationServices)
        {
            this.annotationService = annotationServices.First();
        }

        [EnableCors]
        [HttpGet]
        public ActionResult<IEnumerable<IAnnotationTask>> Get([FromQuery] int number)
        {
            try
            {
                IEnumerable<IAnnotationTask> tasks = annotationService.GetAnnotationTasks(number);
                return Ok(tasks);
            }

            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [EnableCors]
        [HttpGet("practice")]
        public ActionResult<IEnumerable<IAnnotationTask>> GetPractice()
        {
            try
            {
                IEnumerable<IAnnotationTask> practiceTasks = annotationService.GetPracticeTasks();
                return Ok(practiceTasks);
            }

            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [EnableCors]
        [HttpGet("media")]
        public IActionResult GetVideo([FromQuery] string mediaId)
        {
            try
            {
                //this.HttpContext.Response.StatusCode = StatusCodes.Status206PartialContent;
                return annotationService.GetMedia(mediaId);
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
        }

        [EnableCors]
        [HttpPost]
        public ActionResult Post([FromBody] IEnumerable<IAnnotationTask> annotationChunk, [FromQuery] string workerId, [FromQuery] string assignmentId)
        {
            annotationService.RecordAnnotations(annotationChunk, workerId, assignmentId);
            //verify that non empty and return accepted with a guid
            //maybe recordannotations will return a boolean, accepted or not and on the basis of this, we tell the user something
            return Ok();
        }

        [EnableCors]
        [HttpPost("practice")]
        public ActionResult PostPractice([FromBody] IEnumerable<IAnnotationTask> practiceChunk, [FromQuery] String section)
        {
            //Verify and return correct or incorrect
            //should we check one at a time or all at once? I think maybe one at a time...? 
            //not allow people to go on until each one is correct? 
            bool result = annotationService.CheckPractice(practiceChunk.FirstOrDefault(), section);
            return Ok(result);
        }

        [EnableCors]
        [HttpPost("worker")]
        public ActionResult PostWorker([FromQuery] string id, [FromBody] IEnumerable<IAnnotationTask> metadata)
        {
            if (metadata.Count() == 0)
            {
                if (annotationService.HasWorker(id))
                {
                    return Ok();
                }
                else
                {
                    return StatusCode(403);
                }
            }
            else
            {
                annotationService.RecordMetadata(id, metadata);
                return Ok();
            }
            
        }
    }
}
