using ElasticSearch.WebApi.Attributes;

namespace ElasticSearch.WebApi.Domain
{
    [ElasticSearchIndex(IndexName = "user-idx")]
    public record UserEntity : BaseEntity
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public int Age { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
