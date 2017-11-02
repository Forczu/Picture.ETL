using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AnimePictureETL.ExtractedData;
using AnimePictureETL.Repositories;
using AnimePictureETL.Models;
using AnimePictureETL.Scrappers;
using AnimePictureETL.ExtractedData.Danbooru;
using AnimePictureETL.Databases;

namespace AnimePictureETL.Services
{
    public class AnimeWorksServiceImpl : IAnimeWorksService
    {
        private RepositoryBase _repository;

        public AnimeWorksServiceImpl(RepositoryBase repository)
        {
            _repository = repository;
        }

        public void CreateNewCharacters(IList<SubmittedCharacterData> data)
        {
            foreach (var charData in data)
            {
                Character character = new Character
                {
                    Name = charData.Name,
                    FamilyName = charData.FamilyName,
                    DanbooruName = charData.Tag
                };
                try
                {
                    _repository.BeginTransaction();
                    _repository.Save(character);
                    _repository.CommitTransaction();
                }
                catch
                {
                    _repository.RollbackTransaction();
                }
            }
        }

        public Post ScrapDanbooruPicture(string id, string dest)
        {
            int idInt = Convert.ToInt32(id);
            DanbooruScrapper ds = new DanbooruScrapper(dest);
            Post dpe = ds.ExtractPictureById(idInt);
            return dpe;
        }

        public IList<Picture> SearchPictures(string name, string familyName, string series, string artist)
        {
            using (PictureRepository repo = new PictureRepository())
            {
                IList<Picture> pictures = repo.SearchPictures(name, familyName, series, artist);
                return pictures;
            }
        }

        public IList<Picture> GetLastestPictures()
        {
            using (PictureRepository repo = new PictureRepository())
            {
                var pictures = repo.GetLastestPictures(1, 50);
                return pictures;
            }
        }

        public Picture GetPictureForView(long id)
        {
            var session = AnimeWorksDatabase.OpenSession();
            var picture = session.Get<Picture>(id);
            return picture;
        }
    }
}