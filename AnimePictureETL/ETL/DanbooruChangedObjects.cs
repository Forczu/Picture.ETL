using AnimePictureETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.ETL
{
    public class DanbooruChangedObjects
    {
        public IList<Artist> ChangedArtists { get; set; }

        public IList<Character> ChangedCharacters { get; set; }

        public IList<Picture> ChangedPictures { get; set; }

        public IList<CharacterTag> ChangedTags { get; set; }

        public DanbooruChangedObjects()
        {
            ChangedArtists = new List<Artist>();
            ChangedCharacters = new List<Character>();
            ChangedPictures = new List<Picture>();
            ChangedTags = new List<CharacterTag>();
        }
    }
}