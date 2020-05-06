using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CrowdSourcing.DataContracts;
using CrowdSourcing.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace CrowdSourcing.Core
{
    public interface IAnnotationService
    {
        IEnumerable<IAnnotationTask> GetAnnotationTasks(int numberOfTasks);

        IEnumerable<IAnnotationTask> GetPracticeTasks();

        IActionResult GetMedia(string mediaId);

        bool HasWorker(string workerId);

        void RecordAnnotations(IEnumerable<IAnnotationTask> annotation, string workerId, string assignmentId);

        void RecordMetadata(string id, IEnumerable<IAnnotationTask> metadata);

        bool CheckPractice(IAnnotationTask practiceChunk, String section);
    }
}
