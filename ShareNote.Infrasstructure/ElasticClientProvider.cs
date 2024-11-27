using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using ShareNote.Domain.Entities;
namespace ShareNote.Infrasstructure
{
    public class ElasticClientProvider
    {
        private readonly ElasticClient _client;

        public ElasticClientProvider()
        {
            var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("webpages") // Default index name
                .DefaultMappingFor<WebPage>(m => m
                    .PropertyName(p => p.Url, "url")
                    .PropertyName(p => p.Title, "title")
                    .PropertyName(p => p.Content, "content")
                    .PropertyName(p => p.Tags, "tags")
                    .PropertyName(p => p.UpdatedAt, "updated_at")
                    .PropertyName(p => p.Popularity, "popularity")
                );

            _client = new ElasticClient(settings);
        }

        public ElasticClient GetClient() => _client;
    }
}
