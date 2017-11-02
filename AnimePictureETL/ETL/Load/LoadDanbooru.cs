using AnimePictureETL.Models;
using AnimePictureETL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.ETL.Load
{
    public class LoadDanbooru
    {
        private LoadDanbooru()
        {
        }

        public static LoadDanbooru GetInstance()
        {
            return new LoadDanbooru();
        }

        public void LoadPicture(Picture picture)
        {
            using (RepositoryBase repo = new RepositoryBase())
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