using MySql.Data.MySqlClient; 
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RabbitMQ.Client;

namespace DAO_IdentityService.Test
{    
    public class settings{
        public string DbConnectionString { get; set; }
    }
    
    public static class DAL{
        public static string connectionString = "";
        
        static MySqlConnection conn;
        static settings _settings;

        static DAL(){
            _settings = new settings();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            var cs = config.GetSection("PlatformSettings");
            cs.Bind(_settings);            
        }

        static bool ExecuteCommand(string command){            
            using (conn = new MySqlConnection(_settings.DbConnectionString)){
                MySqlCommand comm = new MySqlCommand(command, conn);
                conn.Open();
                var res = comm.ExecuteNonQuery();
                return res==0;
            }         
        }

        ///Email activation 
        public static bool ActivateUser(string Email){
            return ExecuteCommand("update Users set IsActive = 1 where Email = '" + Email + "'");            
        }

        ///KYC verification
        public static bool ActivateKYC(string Email){
            return ExecuteCommand("update Users set KYCStatus = 1 where Email = '" + Email + "'");
        }

        ///Truncating Users table 
        public static bool ClearUsersTable(){
            return ExecuteCommand("truncate table Users");            
        }

        ///Blocks user
        public static bool BlockUser(string Email){
            return ExecuteCommand("update Users set IsActive = 1, KYCStatus = 1, IsBlocked = 1 where Email = '" + Email + "'");            
        }

        ///Resets user to initial register state
        public static bool ResetUser(string Email){
            return ExecuteCommand("update Users set IsActive = 0, KYCStatus = 0, IsBlocked = 0 where Email = '" + Email + "'");
        }


    }
    
}