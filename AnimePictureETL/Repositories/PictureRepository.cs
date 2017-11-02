using AnimePictureETL.Models;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AnimePictureETL.Repositories
{
    public class PictureRepository : RepositoryBase
    {
        public enum SourceType
        {
            Pixiv, Danbooru, Minitokyo
        }

        public bool DoPictureExists(string checksum)
        {
            var pictures = _session.QueryOver<Picture>().Where(a => a.Md5Checksum == checksum).List();
            return pictures.Count != 0;
        }
        
        public bool DoPictureExists(long id, SourceType type)
        {
            Picture picture = GetBySourceId(id, type);
            return picture != null;
        }

        public Picture GetBySourceId(long id, SourceType type)
        {
            Picture pic = null;
            var queryStart = _session.QueryOver<Picture>()
                .JoinQueryOver(n => n.Sources);
            IQueryOver<Picture, Source> whereClause;
            switch (type)
            {
                case SourceType.Pixiv:
                    whereClause = queryStart.Where(s => s.PixivId == id);
                    break;
                case SourceType.Danbooru:
                default:
                    whereClause = queryStart.Where(s => s.DanbooruId == id);
                    break;
                case SourceType.Minitokyo:
                    whereClause = queryStart.Where(s => s.MinitokyoId == id);
                    break;
            }
            pic = whereClause.Take(1).SingleOrDefault();
            return pic;
        }

        public IList<Picture> SearchPictures(string name, string familyName, string series, string artist)
        {
            var restritcions = Restrictions.Conjunction();
            if (!string.IsNullOrEmpty(name))
            {
                restritcions.Add(Restrictions.Like(Projections.Property<Character>(c => c.Name), name));
            }

            Picture pictureAlias = null;
            Artist artistAlias = null;
            Series seriesAlias = null;
            Character characterAlias = null;

            var query = _session.QueryOver<Picture>(() => pictureAlias)
                .Inner.JoinAlias(() => pictureAlias.Artists, () => artistAlias)
                .Inner.JoinAlias(() => pictureAlias.Characters, () => characterAlias);

            if (!string.IsNullOrEmpty(artist))
            {
                query = query.WhereRestrictionOn(() => artistAlias.Name).IsLike("%" + artist + "%");
            }
            if (!string.IsNullOrEmpty(series))
            {
                query = query.WhereRestrictionOn(() => seriesAlias.Name).IsLike("%" + series + "%");
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.WhereRestrictionOn(() => characterAlias.Name).IsLike("%" + name + "%");
            }
            if (!string.IsNullOrEmpty(familyName))
            {
                query = query.WhereRestrictionOn(() => characterAlias.FamilyName).IsLike("%" + familyName + "%");
            }
            var resultSet = query.Take(20).List();
            return resultSet;
        }

        public IList<Picture> GetLastestPictures(int page, int pageSize)
        {
            var pictures = _session.QueryOver<Picture>().OrderBy(p => p.UploadDate).Desc.Skip((page - 1) * pageSize).Take(pageSize).List();
            return pictures;
        }
    }
}