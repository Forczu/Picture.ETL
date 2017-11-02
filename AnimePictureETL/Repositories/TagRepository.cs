using AnimePictureETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Repositories
{
    public class TagRepository : RepositoryBase
    {
        public Tag GetByName(string name)
        {
            Tag obj = _session.QueryOver<Tag>().Where(a => a.Name == name).Take(1).SingleOrDefault();
            return obj;
        }
    }
}