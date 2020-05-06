namespace CrowdSourcing.DataContracts
{
    public interface IMedia : IPropertyBag
    {
        string Content { get; set; }

        byte[] Bytes();
    }
}
