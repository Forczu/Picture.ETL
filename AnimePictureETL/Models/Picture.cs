using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Models
{
    public class Picture
    {
        public virtual long PictureId { get; set; }

        public virtual ISet<Character> Characters { get; set; }

        public virtual ISet<Artist> Artists { get; set; }

        public virtual ISet<Series> Series { get; set; }

        public virtual ISet<CharacterTag> CharacterTags { get; set; }

        public virtual DateTime UploadDate { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual long Size { get; set; }

        public virtual short Width { get; set; }

        public virtual short Height { get; set; }

        public virtual string Source { get; set; }

        public virtual string FileName { get; set; }

        public virtual string Md5Checksum { get; set; }

        public virtual Source Sources { get; set; }
    }
}