using AnimePictureETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnimePictureETL.Repositories
{
    public class CharacterRepository : RepositoryBase
    {
        public Character GetByName(string name)
        {
            Character artist = _session.QueryOver<Character>().Where(a => a.Name == name).Take(1).SingleOrDefault();
            return artist;
        }

        public Character GetByNameAndFamilyName(string name, string familyName)
        {
            Character artist = _session.QueryOver<Character>().Where(a => a.Name == name && a.FamilyName == familyName).Take(1).SingleOrDefault();
            return artist;
        }

        public Character GetByDanbooruName(string nameTag)
        {
            Character character = _session.QueryOver<Character>().Where(a => a.DanbooruName == nameTag).Take(1).SingleOrDefault();
            return character;
        }

        public bool DoExistsByDanbooruName(string name)
        {
            bool result = _session.QueryOver<Character>().Where(a => a.DanbooruName == name).Take(1).SingleOrDefault() != null;
            return result;
        }
    }
}