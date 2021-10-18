using DAO_DbService.Contexts;
using DAO_DbService.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAO_DbService.Test
{
    public class PostUnitTestController
    {
        private UsersController usersControllers;
        public static DbContextOptions<dao_maindb_context> options { get; }
        public static string connectionString = "Server=localhost;Port=3309;Database=daodb_test;Uid=root;Pwd=secred;";

        static PostUnitTestController()
        {
            options = new DbContextOptionsBuilder<dao_maindb_context>()
                .UseMySQL(connectionString)
                .Options;
        }

        public PostUnitTestController()
        {
            var context = new dao_maindb_context(options);
            
            DummyDataDBInitializer db = new DummyDataDBInitializer();
            db.Seed(context);

            usersControllers = new UsersController();
        }

    }
}
