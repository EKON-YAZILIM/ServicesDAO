using MySql.Data.MySqlClient; 
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RabbitMQ.Client;

namespace DAO_IdentityService.Test
{    
    public class settings{
        public string DbConnectionString { get; set; }
    }
    public interface IRabbitMQConnectionFactory {
        IConnection CreateConnection();
    }
    // public class RabbitMQConnection : IRabbitMQConnectionFactory {
    //     private readonly RabbitMQConnectionDetail connectionDetails;
        
    //     public RabbitMQConnection(IOptions<RabbitMQConnectionDetail> connectionDetails) {
    //         this.connectionDetails = connectionDetails.Value;
    //     }

    //     public IConnection CreateConnection() {
    //         var factory = new ConnectionFactory {
    //             HostName = connectionDetails.HostName,
    //             UserName = connectionDetails.UserName,
    //             Password = connectionDetails.Password
    //         };
    //         var connection = factory.CreateConnection();
    //         return connection;
    //     }
    // }
    public static class DAL{
        public static string connectionString = "";
        
        static MySqlConnection conn;
        static settings _settings;

        static DAL(){
            _settings = new settings();
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
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

        public static bool ActivateUser(string Email){
            bool result = false;
            string commandString = "";
            using(conn = new MySqlConnection(_settings.DbConnectionString)){
                MySqlCommand comm = new MySqlCommand(commandString,conn);
                conn.Open();


            }
            return result;
        }

        public static bool ActivateKYC(string email){
            bool result = false;
            return result;
        }

        public static bool ClearUsersTable(){
            return ExecuteCommand("truncate table Users");            
        }
    }
    
}