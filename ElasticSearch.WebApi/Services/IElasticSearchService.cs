using Elastic.Clients.Elasticsearch;
using ElasticSearch.WebApi.Domain;

namespace ElasticSearch.WebApi.Services
{
    public interface IElasticSearchService<T>
        where T : BaseEntity
    {
        Task<bool> AddOrUpdateAsync(T data, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<T>> GetAllAsync(int from = 0, int size = 20, CancellationToken cancellationToken = default);
        Task<T?> GetAsync(Id id, CancellationToken cancellationToken = default);
        Task<IReadOnlyCollection<T>> SearchAsync(Action<SearchRequestDescriptor<T>> configureRequest, CancellationToken cancellationToken = default);
    }
}
