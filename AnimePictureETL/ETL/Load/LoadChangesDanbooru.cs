using AnimePictureETL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.ETL.Load
{
    public class LoadChangesDanbooru
    {
        DanbooruChangedObjects _changedObjects = null;
        
        public void Load(DanbooruChangedObjects changedObjects)
        {
            _changedObjects = changedObjects;
            LoadPictures();
            _changedObjects = null;
        }

        private void LoadPictures()
        {
            foreach (var picture in _changedObjects.ChangedPictures)
            {
                using(RepositoryBase repo = new RepositoryBase())
                {
                    try
                    {
                        repo.BeginTransaction();
                        repo.Save(picture);
                        repo.CommitTransaction();
                    }
                    catch
                    {
                        repo.RollbackTransaction();
                    }
                }
            }
        }
    }
}