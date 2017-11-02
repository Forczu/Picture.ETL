using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.ExtractedData.Danbooru
{
    public sealed class CharacterData
    {
        public bool Exists { get; set; }

        public string Tag { get; set; }

        public string SuggestedName { get; set; }

        public string SuggestedFamilyName { get; set; }
    }
}