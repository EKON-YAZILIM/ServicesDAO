using Helpers.Models.SharedModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySql.Data.MySqlClient;
using System;

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

        /// <summary>
        ///  Generic database creation and migration method
        /// </summary>
        /// <param name="db">Database object of EntityFramework</param>
        /// <returns></returns>
        public ApplicationStartResult Migrate(DatabaseFacade db)
        {
            try
            {
                db.Migrate();
                db.EnsureCreated();

                return new ApplicationStartResult() { Success = true };
            }
            catch (Exception ex)
            {
                return new ApplicationStartResult() { Success = false, Exception = ex };
            }
        }
    }
}
