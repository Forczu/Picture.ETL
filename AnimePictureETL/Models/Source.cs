using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Models
{
    public class Source
    {
        public virtual long SourceId { get; set; }

        public virtual Picture Picture { get; set; }

        public virtual long PixivId { get; set; }

        public virtual long DanbooruId { get; set; }

        public virtual int MinitokyoId { get; set; }
    }
}