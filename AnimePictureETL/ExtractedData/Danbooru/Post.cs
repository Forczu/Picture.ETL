using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AnimePictureETL.ExtractedData.Danbooru
{
    public class Post
    {
        private static char COMMA = ',';
        private static char SEMICOLON = ';';
        
        public bool Exists { get; set; }

        public long DanbooruId { get; set; }

        public string[] Artists { get; set; }

        public string[] Characters { get; set; }

        public string[] Series { get; set; }

        public string[] Tags { get; set; }

        public long Size { get; set; }

        public short Width { get; set; }

        public short Height { get; set; }

        public string Source { get; set; }

        public string FileName { get; set; }

        public string Checksum { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Exists.ToString() + COMMA);
            sb.Append(TabToString(Artists));
            sb.Append(TabToString(Characters));
            sb.Append(TabToString(Series));
            sb.Append(TabToString(Tags));
            sb.Append(Convert.ToInt64(Size) + COMMA);
            sb.Append(Convert.ToInt16(Width) + COMMA);
            sb.Append(Convert.ToInt16(Height) + COMMA);
            sb.Append(Source + COMMA);
            sb.Append(FileName + COMMA);
            sb.Append(Checksum);
            return sb.ToString();
        }

        private string TabToString(string[] tab)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tab.Length; ++i)
            {
                sb.Append(tab[i]);
                if (i != tab.Length - 1)
                    sb.Append(SEMICOLON);
                else
                    sb.Append(COMMA);
            }
            return sb.ToString();
        }
    }
}