using Elastic.Clients.Elasticsearch;
using ElasticSearch.WebApi.Attributes;
using ElasticSearch.WebApi.Domain;
using System.Reflection;

namespace ElasticSearch.WebApi.Services.Base
{
    public class ElasticSearchManager<T> : IElasticSearchService<T>
        where T : BaseEntity
    {
        private readonly ElasticsearchClient _client;
        private readonly string _indexName;

        public ElasticSearchManager(ElasticsearchClient client)
        {
            _client = client;

            ElasticSearchIndexAttribute elasticSearchIndexAttribute = typeof(T).GetCustomAttribute<ElasticSearchIndexAttribute>(true)!;
            _indexName = elasticSearchIndexAttribute.IndexName;
        }

        public async Task<T?> GetAsync(Id id, CancellationToken cancellationToken = default)
        {
            var response = await _client.GetAsync<T>(new GetRequest(_indexName, id), cancellationToken);
            return response.Source;
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(int from = 0, int size = 20, CancellationToken cancellationToken = default)
        {
            //Example
            //List<SortOptions> optionsList = [SortOptions.Field("id", new FieldSort() { Order = SortOrder.Asc }),
            //                                 SortOptions.Field("age", new FieldSort() { Order = SortOrder.Desc })];

            List<SortOptions> optionsList = [SortOptions.Field("id", new FieldSort() { Order = SortOrder.Desc })];

            var response = await _client.SearchAsync<T>(s => s.From(from)
                                                            .Size(size)
                                                            .Sort(optionsList), cancellationToken);
            return response.Documents;
        }

        public async Task<IReadOnlyCollection<T>> SearchAsync(Action<SearchRequestDescriptor<T>> configureRequest, CancellationToken cancellationToken = default)
        {
            //Example:
            //SearchResponse<UserEntity> response1 = await _client.SearchAsync<UserEntity>(s => s
            //    .Query(q => q.Bool(b => b.Must(mm => mm.Match(m => m.Field(u => u.FirstName).Query("Nijat")),
            //                                   mm => mm.QueryString(c => c.Query("Mardan*").DefaultField(u => u.LastName)))
            //                             .Filter(f => f.Range(r => r.NumberRange(n => n.Field(u => u.Age).Gt(20)))))), cancellationToken);

            SearchResponse<T> response = await _client.SearchAsync(configureRequest, cancellationToken);
            return response.Documents;
        }

        public async Task<bool> AddOrUpdateAsync(T data, CancellationToken cancellationToken = default)
        {
            if (data.Id == 0)
            {
                data.Id = await GetNewIdAsync(cancellationToken);
            }

            var response = await _client.IndexAsync(data, _indexName, data.Id, cancellationToken);
            return response.IsValidResponse;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var response = await _client.DeleteAsync(new DeleteRequest(_indexName, id), cancellationToken);
            return response.IsValidResponse;
        }

        private async Task<int> GetNewIdAsync(CancellationToken cancellationToken)
        {
            IReadOnlyCollection<T> lastData = await GetAllAsync(0, 1, cancellationToken);

            int lastId = lastData.FirstOrDefault()?.Id ?? 0;
            return lastId + 1;
        }
    }
}
