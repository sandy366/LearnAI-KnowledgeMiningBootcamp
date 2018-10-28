using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;

namespace CognitiveSearchBot.Models
{
    public class ImageMapper
    {
        public static SearchHit ToSearchHit(SearchResult hit)
        {
                        
            // Retrives fields from Cognitive Search.

            var description = "Cognitive KeyPhrases: " +
                string.Join(",", (hit.Document["keyphrases"] as string[]));

            var searchHit = new SearchHit
            {
                Key = (string)hit.Document["listingId"],
                Title = (string)hit.Document["description"],
                PictureUrl = (string)hit.Document["thumbnail"],
                Description = (string)description
            };

            object Tags;
            if (hit.Document.TryGetValue("Tags", out Tags))
            {
                searchHit.PropertyBag.Add("Tags", Tags);
            }
            return searchHit;
        }

    }
}