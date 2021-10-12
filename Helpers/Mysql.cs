using Helpers.Models.SharedModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Helpers
{
    /// <summary>
    ///  Generic MySql utility class
    /// </summary>
    public class Mysql
    {
        /// <summary>
        ///  Connection tester for MySql
        /// </summary>
        /// <param name="sqlconnectionstring">MySql server connection string</param>
        /// <returns>ApplicationStartResult</returns>
        public ApplicationStartResult Connect(string sqlconnectionstring)
        {
            try
            {

                using (MySqlConnection connection = new MySqlConnection(sqlconnectionstring))
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (MySqlException ex)
                    {
                        return new ApplicationStartResult() { Success = false, Exception = ex };
                    }
                }

                return new ApplicationStartResult() { Success = true };
            }
            catch (Exception ex)
            {
                return new ApplicationStartResult() { Success = false, Exception = ex };
            }
        }

    
    }
}
