using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.ExtractedData.Danbooru
{
    public class TagData
    {
        public bool Exists { get; set; }

        public string Tag { get; set; }

        public string SuggestedTag { get; set; }

        public List<string> CharacterTag { get; set; }
    }
}