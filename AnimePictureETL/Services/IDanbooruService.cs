using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnimePictureETL.ExtractedData.Danbooru;

namespace AnimePictureETL.Services
{
    public interface IDanbooruService
    {
        Post ExtractPost(long id);
        PostChangesView GetSuggestedValuesForPost(Post post);
        void Save(PostChangesView data);
    }
}