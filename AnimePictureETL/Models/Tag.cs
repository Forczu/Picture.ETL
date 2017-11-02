using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Models
{
    public class Tag
    {
        public virtual int TagId { get; set; }

        public virtual string Name { get; set; }

        public virtual ISet<CharacterTag> CharacterTags { get; set; }
    }
}