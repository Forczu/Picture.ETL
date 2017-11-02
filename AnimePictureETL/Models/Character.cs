using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Models
{
    public class Character
    {
        public virtual int CharacterId { get; set; }

        public virtual string Name { get; set; }

        public virtual string FamilyName { get; set; }

        public virtual ISet<Picture> Pictures { get; set; }

        public virtual ISet<CharacterTag> CharacterTags { get; set; }

        public virtual string DanbooruName { get; set; }
    }
}