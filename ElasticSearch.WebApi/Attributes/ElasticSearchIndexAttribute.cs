namespace ElasticSearch.WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ElasticSearchIndexAttribute : Attribute
    {
        public string IndexName { get; set; } = default!;
    }
}
