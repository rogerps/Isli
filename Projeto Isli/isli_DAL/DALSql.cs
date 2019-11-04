using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace isli_DAL
{
    public class DALSql : IDALSql
    {
        public string status { get; set; }
        public string conStr { get; set; }
        
        internal DALSql()
        {
            conStr = Conexao.GetConexao();
        }

        public void transactBD(string query)
        {
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = (query);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                status = ("OK");
            }
            catch (Exception ex)
            {
                status = (ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable select(string query_)
        {
            DataTable _tbl = new DataTable();
            SqlConnection cn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand(query_, cn);
            try
            {
                cn.Open();
                _tbl.Load(cmd.ExecuteReader());
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            finally
            {
                cn.Close();
            }
            return _tbl;
        }

        public int count(string query_)
        {
            int count = 0;
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = (query_);
            try
            {
                conn.Open();
                count = Convert.ToInt32(cmd.ExecuteScalar());
                status = ("OK");
            }
            catch (Exception ex)
            {
                status = ("Falha ao realizar o COUNT. A mensagem retornada foi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return count;
        }

        public string executeScalar(string query_)
        {
            string _valor = "";
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = (query_);
            try
            {
                conn.Open();
                _valor = Convert.ToString(cmd.ExecuteScalar()).Trim();
                status = ("OK");
            }
            catch (Exception ex)
            {
                status = ("Falha ao realizar a operação. A mensagem retornada foi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return _valor;
        }

        public void executeMultipleProcedure(string procedure_, List<SqlParameter[]> listaParametros)
        {
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = (procedure_);

            try
            {
                conn.Open();
                foreach (SqlParameter[] parametros in listaParametros)
                {
                    cmd.Parameters.AddRange(parametros);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                status = "OK";
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }

        public void executeBulkInsert(DataTable dados, string tabela, int tamanhoLote = 1000, SqlBulkCopyOptions sqlBulkCopyOptions = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.FireTriggers | SqlBulkCopyOptions.UseInternalTransaction)
        {
            SqlConnection conn = new SqlConnection(conStr);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, sqlBulkCopyOptions, null);

            bulkCopy.DestinationTableName = tabela;

            try
            {
                conn.Open();

                //conta quantas particoes de tamanho 'tamanhoLote' existem
                int particoes = (int)Math.Floor(dados.Rows.Count / tamanhoLote * 1.0);


                for (int i = 0; i < particoes; i++)
                {
                    DataTable particao = dados.AsEnumerable().Skip((i * tamanhoLote)).Take(tamanhoLote).CopyToDataTable();
                    bulkCopy.WriteToServer(particao);
                }

                if (particoes * tamanhoLote < dados.Rows.Count)
                {
                    DataTable remanscente = dados.AsEnumerable().Skip(particoes * tamanhoLote).Take(tamanhoLote + 1).CopyToDataTable();
                    bulkCopy.WriteToServer(remanscente);
                }
                status = "OK";
            }
            catch (Exception ex)
            {
                status = ex.Message;
                // Mensagem de erro da classe SqlBulkCopy. Este código auxilia na melhoria desta mensagem mostrando qual coluna
                // exatamente está com o tamanho inválido
                if (ex.Message.Contains("Received an invalid column length from the bcp client for colid"))
                {
                    string pattern = @"\d+";
                    Match match = Regex.Match(ex.Message.ToString(), pattern);
                    var index = Convert.ToInt32(match.Value) - 1;

                    FieldInfo fi = typeof(SqlBulkCopy).GetField("_sortedColumnMappings", BindingFlags.NonPublic | BindingFlags.Instance);
                    var sortedColumns = fi.GetValue(bulkCopy);
                    var items = (Object[])sortedColumns.GetType().GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sortedColumns);

                    FieldInfo itemdata = items[index].GetType().GetField("_metadata", BindingFlags.NonPublic | BindingFlags.Instance);
                    var metadata = itemdata.GetValue(items[index]);

                    var column = metadata.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    var length = metadata.GetType().GetField("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(metadata);
                    status = String.Format("Column: {0} contains data with a length greater than: {1}", column, length);
                }
            }
            finally
            {
                conn.Close();
            }
        }

        public void executeProcedure(string procedure_, SqlParameter[] parametros)
        {
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = (procedure_);
            cmd.Parameters.AddRange(parametros);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                status = "OK";
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            finally
            {
                conn.Close();
            }
        }

        public DataTable selectProcedure(string procedure_, SqlParameter[] parametros)
        {

            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = (procedure_);
            cmd.Parameters.AddRange(parametros);
            DataTable _tbl = new DataTable();
            try
            {
                conn.Open();
                _tbl.Load(cmd.ExecuteReader());
                status = "OK";
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return _tbl;
        }

        public DataTable selectProcedure(string procedure_, SqlParameter parametro)
        {

            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = (procedure_);
            cmd.Parameters.Add(parametro);
            DataTable _tbl = new DataTable();
            try
            {
                conn.Open();
                _tbl.Load(cmd.ExecuteReader());
                status = "OK";
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return _tbl;
        }

        public DataTable selectProcedure(string procedure_)
        {
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = (procedure_);
            DataTable _tbl = new DataTable();
            try
            {
                conn.Open();
                _tbl.Load(cmd.ExecuteReader());
                status = "OK";
            }
            catch (Exception ex)
            {
                status = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return _tbl;
        }

        public string execProcedureScalar(string procedure_, SqlParameter[] parametros)
        {
            SqlConnection conn = new SqlConnection(conStr);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = (procedure_);
            cmd.Parameters.AddRange(parametros);

            string valor = "0";
            try
            {
                conn.Open();
                valor = Convert.ToString(cmd.ExecuteScalar());
                status = "OK";
            }
            catch (Exception ex)
            {
                valor = "0";
                status = ex.Message;
            }
            finally
            {
                conn.Close();
            }
            return valor;
        }
    }
}
