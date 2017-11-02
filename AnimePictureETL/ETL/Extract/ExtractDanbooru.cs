using AnimePictureETL.ExtractedData.Danbooru;
using AnimePictureETL.Scrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.ETL.Extract
{
    public class ExtractDanbooru
    {
        private DanbooruScrapper scrapper = new DanbooruScrapper();

        private ExtractDanbooru()
        {
        }

        public static ExtractDanbooru GetInstance()
        {
            return new ExtractDanbooru();
        }

        public Post ExtractPost(long id)
        {
            var post = scrapper.ExtractPictureById(id);
            return post;
        }
    }
}