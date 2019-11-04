using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isli_DAL
{
    public static class FactoryDAL
    {
        public static IDALSql CreateDacSQL()
        {
            return new DALSql();
        }
    }
}
