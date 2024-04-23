using Elastic.Clients.Elasticsearch;
using ElasticSearch.WebApi.Services;
using ElasticSearch.WebApi.Services.Base;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

namespace ElasticSearch.WebApi
{
    public static class ServiceRegistration
    {

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            string elasticSearchUri = configuration["ElasticConfiguration:Uri"]!;

            services.AddSingleton(s => new ElasticsearchClient(new Uri(elasticSearchUri)));

            services.AddSingleton(typeof(IElasticSearchService<>), typeof(ElasticSearchManager<>));

            return services;
        }

        public static WebApplicationBuilder SetupWebApplicationBuilder(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            string elasticSearchUri = configuration["ElasticConfiguration:Uri"]!;

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticSearchUri))
                {
                    AutoRegisterTemplate = true,
                    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name!.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
                    NumberOfReplicas = 1,
                    NumberOfShards = 2
                })
                .CreateLogger();

            builder.Logging.AddSerilog();

            return builder;
        }
    }
}
