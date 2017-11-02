using AnimePictureETL.ExtractedData;
using AnimePictureETL.ExtractedData.Danbooru;
using AnimePictureETL.Models;
using AnimePictureETL.Scrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Clients
{
    public class DanbooruClient
    {
        private DanbooruScrapper _scrapper = null;

        public DanbooruClient()
        {
            _scrapper = new DanbooruScrapper("");
        }

        public IList<Post> GetPicturesByTag(string tag)
        {
            return _scrapper.ExtractPicturesByTag(tag);
        }

    }
}