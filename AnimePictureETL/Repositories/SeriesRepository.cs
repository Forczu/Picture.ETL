using AnimePictureETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Repositories
{
    public class SeriesRepository : RepositoryBase
    {
        public Series GetByName(string name)
        {
            Series series = _session.QueryOver<Series>().Where(a => a.Name == name).Take(1).SingleOrDefault();
            return series;
        }
    }
}