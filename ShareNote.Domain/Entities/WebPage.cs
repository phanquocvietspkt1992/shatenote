using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareNote.Domain.Entities
{
    public class WebPage
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<string> Tags { get; set; }
        public DateTime UpdatedAt { get; set; }
        public float Popularity { get; set; }
    }

}
