using AnimePictureETL.ExtractedData;
using AnimePictureETL.ExtractedData.Danbooru;
using AnimePictureETL.Scrappers;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Web;

namespace AnimePictureETL.Extract.ETL
{
    public class ExtractChangesDanbooru
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private IList<string> _observedTags = new List<string>();

        private DanbooruScrapper scrapper =  null;

        private string _downloadDest = null;

        public ExtractChangesDanbooru(IList<string> observedTags, string downloadDest)
        {
            _observedTags = observedTags;
            _downloadDest = downloadDest;
            scrapper = new DanbooruScrapper(downloadDest);
        }

        public IList<Post> Extract()
        {
            IList<Post> changes = new List<Post>(), newTagChanges;
            List<Post> thirdList;
            foreach (var tag in _observedTags)
            {
                log.Info("Getting the changes for tag: " + tag);
                newTagChanges = scrapper.ExtractPicturesByTag(tag);
                thirdList = new List<Post>(changes.Count + newTagChanges.Count);
                thirdList.AddRange(changes);
                thirdList.AddRange(newTagChanges);
                changes = thirdList;
            }
            return changes;
        }
    }
}