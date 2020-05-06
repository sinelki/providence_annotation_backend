namespace CrowdSourcing.DataContracts
{
    public interface ITranscriptElement : IPropertyBag
    {
        string Id { get; set; }

        string Phrase { get; set; }

        string Author { get; set; }
    }
}
