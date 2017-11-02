using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimePictureETL.Repositories
{
    public interface IRepository
    {
        void Save(object entity);
        void Delete(object entity);
        object GetById(Type objType, object objId);
    }
}
