﻿namespace Leto2bot.Modules.Searches.Models
{
    public class WikipediaApiModel
    {
        public WikipediaQuery Query { get; set; }

        public class WikipediaQuery
        {
            public WikipediaPage[] Pages { get; set; }

            public class WikipediaPage
            {
                public bool Missing { get; set; } = false;
                public string FullUrl { get; set; }
            }
        }
    }
}