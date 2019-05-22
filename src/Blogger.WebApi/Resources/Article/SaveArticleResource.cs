using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Blogger.WebApi.Resources.Article
{
    public class SaveArticleResource
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; }

        [Required]
        [MinLength(5)]
        public string Description { get; set; }

        [Required]
        [MinLength(10)]
        public string Body { get; set; }

        [JsonProperty("tags")]
        public List<string> TagList { get; set; }
    }
}