using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Models
{
    public class Artist
    {
        public virtual int ArtistId { get; set; }

        public virtual string Name { get; set; }

        public virtual ISet<Picture> Pictures { get; set; }
    }
}