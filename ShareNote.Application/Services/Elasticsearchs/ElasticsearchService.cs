using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShareNote.Domain.Entities;
using ShareNote.Infrasstructure;
using Nest;

namespace ShareNote.Application.Services.Elasticsearchs
{
    public class ElasticsearchService
    {
        private readonly ElasticClient _client;

        public ElasticsearchService(ElasticClientProvider clientProvider)
        {
            _client = clientProvider.GetClient();
        }

        public void CreateIndex()
        {
            var createIndexResponse = _client.Indices.Create("webpages", c => c
                .Map<WebPage>(m => m
                    .AutoMap()
                    .Properties(props => props
                        .Text(t => t.Name(n => n.Title).Analyzer("standard"))
                        .Text(t => t.Name(n => n.Content).Analyzer("english"))
                        .Keyword(k => k.Name(n => n.Url))
                        .Date(d => d.Name(n => n.UpdatedAt))
                        .Number(n => n.Name(n => n.Popularity).Type(NumberType.Float))
                        .Keyword(k => k.Name(n => n.Tags))
                    )
                )
            );

            if (!createIndexResponse.IsValid)
            {
                throw new Exception($"Failed to create index: {createIndexResponse.ServerError}");
            }
        }
        public void IndexDocument(WebPage page)
        {
            var response = _client.IndexDocument(page);

            if (!response.IsValid)
            {
                throw new Exception($"Failed to index document: {response.ServerError}");
            }
        }
        public void BulkIndexDocuments(IEnumerable<WebPage> pages)
        {
            var response = _client.Bulk(b => b
                .IndexMany(pages)
            );

            if (!response.IsValid)
            {
                throw new Exception($"Bulk indexing failed: {response.ServerError}");
            }
        }
        public IEnumerable<WebPage> Search(string query)
        {
            var response = _client.Search<WebPage>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Content)
                        .Query(query)
                    )
                )
            );

            if (!response.IsValid)
            {
                throw new Exception($"Search failed: {response.ServerError}");
            }

            return response.Documents;
        }
        public IEnumerable<WebPage> FilteredSearch(string query, DateTime updatedAfter)
        {
            var response = _client.Search<WebPage>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Match(mt => mt
                                .Field(f => f.Content)
                                .Query(query)
                            )
                        )
                        .Filter(f => f
                            .DateRange(dr => dr
                                .Field(f => f.UpdatedAt)
                                .GreaterThanOrEquals(updatedAfter)
                            )
                        )
                    )
                )
            );

            if (!response.IsValid)
            {
                throw new Exception($"Filtered search failed: {response.ServerError}");
            }

            return response.Documents;
        }
        public IEnumerable<WebPage> SortedSearch(string query)
        {
            var response = _client.Search<WebPage>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Content)
                        .Query(query)
                    )
                )
                .Sort(so => so
                    .Descending(d => d.Popularity)
                )
            );

            if (!response.IsValid)
            {
                throw new Exception($"Sorted search failed: {response.ServerError}");
            }

            return response.Documents;
        }
        public IEnumerable<dynamic> HighlightSearch(string query)
        {
            var response = _client.Search<WebPage>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f.Content)
                        .Query(query)
                    )
                )
                .Highlight(h => h
                    .Fields(f => f
                        .Field(fl => fl.Content)
                    )
                )
            );

            if (!response.IsValid)
            {
                throw new Exception($"Highlight search failed: {response.ServerError}");
            }

            return response.Hits.Select(hit => new
            {
                hit.Source,
                Highlight = hit.Highlight["content"]
            });
        }


    }
}
