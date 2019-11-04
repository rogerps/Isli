using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace isli_DAL
{
    public interface IDALSql
    {
        string status { get; set; }
        string conStr { get; set; }

        void transactBD(string query);
        DataTable select(string query_);
        int count(string query_);
        string executeScalar(string query_);
        void executeMultipleProcedure(string procedure_, List<SqlParameter[]> listaParametros);
        void executeBulkInsert(DataTable dados, string tabela, int tamanhoLote = 1000, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction);
        void executeProcedure(string procedure_, SqlParameter[] parametros);
        DataTable selectProcedure(string procedure_, SqlParameter[] parametros);
        DataTable selectProcedure(string procedure_, SqlParameter parametro);
        DataTable selectProcedure(string procedure_);
        string execProcedureScalar(string procedure_, SqlParameter[] parametros);
    }
}
