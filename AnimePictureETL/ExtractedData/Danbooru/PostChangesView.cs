using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.ExtractedData.Danbooru
{
    public class PostChangesView
    {
        public long DanbooruId { get; set; }

        public List<CharacterData> Characters { get; set; }

        public List<ArtistData> Artists { get; set; }

        public List<SeriesData> Series { get; set; }

        public List<TagData> Tags { get; set; }

        public short Width { get; set; }

        public short Height { get; set; }

        public long Size { get; set; }

        public string Source { get; set; }

        public string FileName { get; set; }

        public string Checksum { get; set; }
    }
}