using System.Collections.Generic;

namespace CrowdSourcing.DataContracts
{
    public interface IDataProvider
    {
        IEnumerable<ITranscriptElement> GetNLeastAnnotated(int numberOfTasks);

        IEnumerable<ITranscriptElement> GetPracticeTasks();

        IContextInfo GetContextInfo(ITranscriptElement element);

        IEnumerable<ITranscriptElement> GetContextContent(IContextInfo contextInfo);

        bool AddWorker(string workerId);

        void AddAnnotation(ITranscriptElement transcript, IPropertyBag annotation, string workerId, string assignmentId);

        void AddAnnotatorInformation(string annotator, IPropertyBag metadata);

        IMedia GetMedia(string mediaId);

        IPropertyBag GetAnnotation(string id);
    }
}
