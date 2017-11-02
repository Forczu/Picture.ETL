using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Models
{
    public class CharacterTag
    {
        public virtual long CharacterTagId { get; set; }

        public virtual Picture Picture { get; set; }

        public virtual Tag Tag { get; set; }

        public virtual Character Character { get; set; }
    }
}