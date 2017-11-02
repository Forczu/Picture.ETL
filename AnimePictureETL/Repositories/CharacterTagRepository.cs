using AnimePictureETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Repositories
{
    public class CharacterTagRepository : RepositoryBase
    {
        public Tag GetTagByName(string name)
        {
            Tag tag = _session.QueryOver<Tag>().Where(a => a.Name == name).Take(1).SingleOrDefault();
            return tag;
        }
    }
}