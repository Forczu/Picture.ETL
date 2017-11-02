using AnimePictureETL.ExtractedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnimePictureETL.Models;
using AnimePictureETL.ExtractedData.Danbooru;

namespace AnimePictureETL.Services
{
    public interface IAnimeWorksService
    {
        void CreateNewCharacters(IList<SubmittedCharacterData> data);
        Post ScrapDanbooruPicture(string id, string dest);
        IList<Picture> SearchPictures(string name, string familyName, string series, string artist);
        IList<Picture> GetLastestPictures();
        Picture GetPictureForView(long id);
    }
}