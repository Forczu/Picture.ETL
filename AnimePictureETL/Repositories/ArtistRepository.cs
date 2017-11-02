using AnimePictureETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Repositories
{
    public class ArtistRepository : RepositoryBase
    {
        public Artist GetByName(string name)
        {
            Artist artist = _session.QueryOver<Artist>().Where(a => a.Name == name).Take(1).SingleOrDefault();
            return artist;
        }

        public bool Exists(string name)
        {
            Artist artist = _session.QueryOver<Artist>().Where(a => a.Name == name).Take(1).SingleOrDefault();
            return artist != null;
        }
    }
}