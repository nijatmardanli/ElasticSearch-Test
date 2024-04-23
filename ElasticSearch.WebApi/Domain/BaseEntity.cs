using System.Text.Json;

namespace ElasticSearch.WebApi.Domain
{
    public record BaseEntity
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
