using System;
using System.Data;
using System.Data.SqlClient;

namespace SicoreQMS.Common
{
    public class DBHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        //public static string connStr = ConfigurationManager.ConnectionStrings["MrCy"].ConnectionString;//正式环境
        //public static string connStr = "server=127.0.0.1;port=3306;user=root;password=maiku; database=mesqas";//MySql测试环境
       //public static string connStr = "Server=.;Initial Catalog=SicoreMES;User ID=sa;Password=123456";//SqlServer测试环境
       public static string connStr = "Server=172.16.2.8;Initial Catalog=SicoreQMS;User ID=sa;Password=sicore#123";//SqlServer正式环境

        /// <summary>
        /// 查询方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static DataTable GetDataTable(string sql, params SqlParameter[] param)
        {
            try
            {
                SqlDataAdapter dap = new SqlDataAdapter(sql, connStr);
                if (param != null)
                {
                    dap.SelectCommand.Parameters.AddRange(param);
                }
                ////测试记录日志
                //SimpleLoger.Instance.Info(sql);
                DataTable dt = new DataTable();
                dap.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                //SimpleLoger.Instance.Debug(ex);
                //SimpleLoger.Instance.Error(sql);
                throw ex;
            }
        }

        /// <summary>
        /// 增、删、改方法
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static bool GetExecuteNonQuery(string sql, params SqlParameter[] param)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    if (param != null)
                    {
                        cmd.Parameters.AddRange(param);
                    } 
                    int i = cmd.ExecuteNonQuery();
                    //测试记录日志
                    string result = Get_sql_query(cmd);
                    //SimpleLoger.Instance.Info(result);
                    return i > 0;
                }
            }
            catch (Exception ex)
            {
                //SimpleLoger.Instance.Debug(ex);
                //SimpleLoger.Instance.Error(sql);
                throw ex;
            }
        }
        public static void ExecuteProc(string sql, params SqlParameter[] param)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (param != null)
                    {
                        cmd.Parameters.AddRange(param);
                    }
                    //测试记录日志
                    string result = Get_sql_query(cmd);
                    //SimpleLoger.Instance.Info(result);
                    int i = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //SimpleLoger.Instance.Debug(ex);
                //SimpleLoger.Instance.Error(sql);
                throw ex;
            }
        }
        public static DataSet ExecuteProcRe(string sql, params SqlParameter[] param)
        {
            DataSet dst = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (param != null)
                    {
                        cmd.Parameters.AddRange(param);
                    }
                    SqlDataAdapter dp = new SqlDataAdapter(cmd);
                    dp.Fill(dst);
                   // //测试记录日志
                   // string result = Get_sql_query(cmd);
                    //SimpleLoger.Instance.Info(result);

                }
            }
            catch (Exception ex)
            {
                //SimpleLoger.Instance.Debug(ex);
                //SimpleLoger.Instance.Error(sql);
                throw ex;
            }

            return dst;
        }
        public static DataSet ExecuteProcReturn(string sql, params SqlParameter[] param)
        {
          
            DataSet dst = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (param != null)
                    {
                        cmd.Parameters.AddRange(param);
                    }
                    //测试记录日志
                    //string result = Get_sql_query(cmd);
                    //SimpleLoger.Instance.Info(result);

                    SqlDataAdapter dp = new SqlDataAdapter(cmd);
                    dp.Fill(dst);
                }
            }
            catch (Exception ex)
            {
                //SimpleLoger.Instance.Debug(ex);
                //SimpleLoger.Instance.Error(sql);
                throw ex;
            }
            return dst;
        }

        public static string Get_sql_query(SqlCommand cmd)
        {
            string query = cmd.CommandText;

            foreach (SqlParameter p in cmd.Parameters)
            {
                query = query.Replace(p.ParameterName, p.Value.ToString());
            }
            return query;
        }



    }
}
