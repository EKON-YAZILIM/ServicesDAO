using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RabbitMQ.Client;
using System.Collections.Generic;
using PagedList.Core;
using Stripe;
using Microsoft.Extensions.Localization;
using Microsoft.CodeAnalysis;
using System;
using System.Text;
using DAO_LogService.Models;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace DAO_WebPortal.Test
{
    public class settings
    {
        public string DbConnectionString { get; set; }
        public string LogDbConnectionString {get; set;}
    }

    public static class DAL
    {
        public static string connectionString = "";
        public static string logconnectionString = "";

        static MySqlConnection conn;
        static settings _settings;

        static DAL()
        {
            _settings = new settings();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            var cs = config.GetSection("PlatformSettings");
            cs.Bind(_settings);
        }

        static bool ExecuteCommand(string command)
        {
            using (conn = new MySqlConnection(_settings.DbConnectionString))
            {
                MySqlCommand comm = new MySqlCommand(command, conn);
                conn.Open();
                var res = comm.ExecuteNonQuery();
                return res == 0;
            }
        }

        ///Email activation 
        public static bool ActivateUser(string Email)
        {
            return ExecuteCommand("update Users set IsActive = 1 where Email = '" + Email + "'");
        }

        ///KYC verification
        public static bool ActivateKYC(string Email)
        {
            return ExecuteCommand("update Users set KYCStatus = 1 where Email = '" + Email + "'");
        }

        ///Truncating Users table 
        public static bool ClearUsersTable()
        {
            return ExecuteCommand("truncate table Users");
        }

        ///Blocks user
        public static bool BlockUser(string Email)
        {
            return ExecuteCommand("update Users set IsActive = 1, KYCStatus = 1, IsBlocked = 1 where Email = '" + Email + "'");
        }

        ///Resets user to initial register state
        public static bool ResetUser(string Email)
        {
            return ExecuteCommand("update Users set IsActive = 0, KYCStatus = 0, IsBlocked = 0 where Email = '" + Email + "'");
        }

        static bool LogsExecuteCommand(string command)
        {
            using (conn = new MySqlConnection(_settings.LogDbConnectionString))
            {
                MySqlCommand comm = new MySqlCommand(command, conn);
                conn.Open();
                var res = comm.ExecuteNonQuery();
                return res == 0;
            }
        }
        public static bool ClearLogTables()
        {
            var a = LogsExecuteCommand("truncate table UserLogs");
            var b = LogsExecuteCommand("truncate table ErrorLogs");
            var c = LogsExecuteCommand("truncate table ApplicationLogs");

            return a && b && c;
        }

        
        public static List<object> GetLogsList(string tablename){
            List<object> resultList = new List<object>();
            
            string command = String.Format("SELECT * FROM {0}", tablename);
            using (conn = new MySqlConnection(_settings.LogDbConnectionString))
            {
                MySqlCommand comm2 = new MySqlCommand(command, conn);
                conn.Open();
                var res = comm2.ExecuteNonQuery();

                MySqlDataReader reader = comm2.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        switch(tablename){
                            case "UserLogs":
                            resultList.Add(new UserLog{
                                UserLogId = reader.GetInt32(0),
                                UserId = reader.GetInt32(1),
                                Date = DateTime.Parse(reader.GetString(2)),
                                Type = reader.GetString(3),
                                Explanation = reader.GetString(4)
                            });            
                            break;
                            case "ErrorLog":
                            resultList.Add(new ErrorLog{
                                ErrorLogId = reader.GetInt32(0),
                                Server = reader.GetString(1),
                                Application = reader.GetString(2),
                                Message = reader.GetString(3),
                                Target = reader.GetString(4),
                                Trace = reader.GetString(5),
                                Date = DateTime.Parse(reader.GetString(6)),
                                IdFieldName = reader.GetString(7),
                                IdField = reader.GetInt32(8),
                                Type = reader.GetString(9)
                            });
                            break;
                            case "ApplicarionLog":
                            resultList.Add(new ApplicationLog{
                                Application = reader.GetString(0),
                                Server = reader.GetString(1),
                                Date = DateTime.Parse(reader.GetString(2)),
                                Explanation = reader.GetString(5)
                            });
                            break;   
                        }         
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }
                reader.Close();
            }
            return resultList;
        }



    }

}
