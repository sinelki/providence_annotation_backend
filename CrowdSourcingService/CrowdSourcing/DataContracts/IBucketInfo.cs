namespace CrowdSourcing.DataContracts
{
    public interface IBucketInfo : IPropertyBag
    {
        int Id { get; set; }

        int Begin { get; set; }

        int End { get; set; }

        bool IsValid();
    }
}
