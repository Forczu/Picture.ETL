using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.ExtractedData
{
    public class SubmittedCharacterData
    {
        public virtual string Tag { get; set; }

        public virtual string Name { get; set; }

        public virtual string FamilyName { get; set; }
    }
}