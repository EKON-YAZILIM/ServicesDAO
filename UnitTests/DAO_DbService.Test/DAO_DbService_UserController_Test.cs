using Xunit;
using FluentAssertions;
using Helpers.Models.DtoModels.MainDbDto;
using System;
using DAO_DbService.Models;
using System.Collections.Generic;
using System.Linq;
using PagedList.Core;
using Microsoft.EntityFrameworkCore;
using DAO_DbService.Tests.Util;

namespace DAO_DbService.Tests
{
    ///<summary>
    /// Tests of DAO_DbService.UsersController class methods.
    /// DAO_DbService microservice is only responsible for database CRUD operations.
    /// Methods:
    /// IEnumerable<UserDto> Get()
    /// IEnumerable<UserDto> UserSearch(string query)
    /// UserDto GetId(int id)
    /// UserDto Post([FromBody] UserDto model)
    /// List<UserDto> PostMultiple([FromBody] List<UserDto> model)
    /// bool Delete(int? ID)
    /// UserDto Update([FromBody] UserDto model)
    /// PaginationEntity<UserDto> GetPaged(int page = 1, int pageCount = 30)
    /// PaginationEntity<UserDto> Search(string query, int page = 1, int pageCount = 30)
    /// UserDto GetByEmail(string email)
    /// UserDto GetByUsername(string username)  
    /// </summary>
    [Collection("Sequential")]
    public class DAO_DbService_Test
    {
        PostTestController controllers = new PostTestController();

        // IEnumerable<UserDto> Get() +
        // IEnumerable<UserDto> UserSearch(string query) +
        // UserDto GetId(int id) +
        // UserDto Post([FromBody] UserDto model) +
        // List<UserDto> PostMultiple([FromBody] List<UserDto> model) 
        // bool Delete(int? ID) +
        // UserDto Update([FromBody] UserDto model) +
        // PaginationEntity<UserDto> GetPaged(int page = 1, int pageCount = 30) 
        // PaginationEntity<UserDto> Search(string query, int page = 1, int pageCount = 30)
        // UserDto GetByEmail(string email) +
        // UserDto GetByUsername(string username) +

        /// Test of DAO_DbService.UsersController.Get()
        /// TestDbInitializer creates random number of users and returns the user list
        [Fact]
        public void UsersController_Get_Test()
        {
            //Arrange
            TestDbInitializer.ClearDatabase();
            var users = TestDbInitializer.SeedUsers();
            //Act
            var getUsers = controllers.usersController.Get();
            //Assert
            getUsers.Should().HaveCount(users.Count);
        }

        /// Test of DAO_DbService.UsersController.UserSearch(query)
        /// Tested for 2 use cases :: searching for one existing and one non-existing user        
        [Fact]
        public void UsersController_UserSearch_Test()
        {
            //Arrange
            TestDbInitializer.ClearDatabase();
            TestDbInitializer.SeedUsers();
            string searchkey = "username4";
            //Act
            IEnumerable<UserDto> user = controllers.usersController.UserSearch(searchkey);
            //Assert
            (user.FirstOrDefault().UserName.Contains(searchkey) || user.FirstOrDefault().Email.Contains(searchkey)).Should().Be(true);

            //Arrange
            string wrongSearchKey = "wrongkey";
            //Act
            IEnumerable<UserDto> non_existant_user = controllers.usersController.UserSearch(wrongSearchKey);
            //Assert
            non_existant_user.Should().HaveCount(0);
        }

        /// Test of DAO_DbService.UsersController.GetId(id)
        /// Finds a user by its id
        [Fact]
        public void UsersController_GetId_Test()
        {
            //Arrange
            TestDbInitializer.ClearDatabase();
            var user_list = TestDbInitializer.SeedUsers();
            Random random = new Random();
            int lucky_user = random.Next(1, user_list.Count);
            User user = user_list.ElementAt(lucky_user);
            //Act
            UserDto user_called_by_id = controllers.usersController.GetId(user.UserId);
            //Assert
            user_called_by_id.UserId.Should().Be(user.UserId);
        }

        /// Test of DAO_DbService.UsersController.Post(UserDto)
        /// Adds user to database
        [Fact]
        public void UsersController_Post_Test()
        {
            // Arrange
            // Creates an internal user
            UserDto userDto = new UserDto
            {
                NameSurname = "InternalUserName InternalUserSurname",
                Email = "user@internal.com",
                Password = Util.TestDbInitializer.testPassword,
                Newsletter = true,
                UserType = "Internal",
                IsBlocked = false,
                IsActive = true,
                CreateDate = DateTime.Now,
                KYCStatus = true,
                FailedLoginCount = 0,
                ProfileImage = "image.jpg",
                UserName = "internalusername",
            };
            // Act
            var addedUser = controllers.usersController.Post(userDto);
            // Assert
            addedUser.NameSurname.Should().Be("InternalUserName InternalUserSurname");
        }

        /// Test of DAO_DbService.UsersController.Post(UserDto)
        /// Inserts a set of user models into the database
        [Fact]
        public void UsersController_PostMultiple_Test()
        {
            //Arrange
            TestDbInitializer.ClearDatabase();
            List<UserDto> users_list = new List<UserDto>();
            Random random = new Random();
            var range_volume = random.Next(5, 1000);
            for (int i = 0; i < range_volume; i++)
            {
                users_list.Add(
                    new UserDto
                    {
                        NameSurname = $"Username{i} UserSurname{i}",
                        Email = $"username{i}@public.com",
                        Password = Util.TestDbInitializer.testPassword,
                        Newsletter = true,
                        UserType = "Public",
                        IsBlocked = false,
                        IsActive = true,
                        CreateDate = DateTime.Now,
                        KYCStatus = true,
                        FailedLoginCount = 0,
                        ProfileImage = "image.jpg",
                        UserName = $"public_username{i}",
                    }
                );
            }
            //Act
            var addedList = controllers.usersController.PostMultiple(users_list);
            ///Assert
            addedList.Should().HaveCount(users_list.Count);
        }

        /// Test of DAO_DbService.UsersController.Delete(ID)
        /// Deletes user by Id
        [Fact]
        public void UsersController_DeleteUser_Test()
        {

            //Arrange
            TestDbInitializer.ClearDatabase();
            List<User> users = TestDbInitializer.SeedUsers();
            Random rand = new Random();
            int lucky_user = rand.Next(1, users.Count);
            //Act

            bool result = controllers.usersController.Delete(lucky_user);
            var user_getById = controllers.usersController.GetId(lucky_user);

            //Assert
            (result || user_getById == null).Should().Be(true);
        }


        /// Test of DAO_DbService.UsersController.GetUserByEmail(Email)
        /// Returns user by email
        [Fact]
        public void UsersController_GetUserByEmail_Test()
        {
            Random rand = new Random();

            //Arrange
            TestDbInitializer.ClearDatabase();
            var user_list = TestDbInitializer.SeedUsers();
            int lucky_user = rand.Next(1, user_list.Count());
            User user = user_list.ElementAt(lucky_user);

            //Act
            UserDto user_called_by_Email = controllers.usersController.GetByEmail(user.Email);
            //Assert
            user_called_by_Email.UserName.Should().Be(user.UserName);
        }

        /// Test of DAO_DbService.UsersController.GetUserByUsername(Username)
        /// Returns user by username
        [Fact]
        public void UsersController_GetUserByUserName_Test()
        {
            Random rand = new Random();
            //Arrange
            TestDbInitializer.ClearDatabase();
            var user_list = TestDbInitializer.SeedUsers();
            int lucky_user = rand.Next(1, user_list.Count());
            User user = user_list.ElementAt(lucky_user);

            //Act
            UserDto user_called_by_UserName = controllers.usersController.GetByUsername(user.UserName);
            //Assert
            user_called_by_UserName.Email.Should().Be(user.Email);
        }

        /// Test of DAO_DbService.UsersController.Update(UserDto model)
        /// Updates user model 
        [Fact]
        public void UsersController_Update_Test()
        {
            Random rand = new Random();
            //Arrange
            TestDbInitializer.ClearDatabase();
            var user_list = TestDbInitializer.SeedUsers();
            int lucky_user = rand.Next(1, user_list.Count());
            User user = user_list.ElementAt(lucky_user);
            UserDto user_to_be_updated = controllers.usersController.GetId(user.UserId);

            user_to_be_updated.UserName = "username_is_updated";
            //Act
            UserDto updated_user = controllers.usersController.Update(user_to_be_updated);
            //Assert
            (updated_user.UserId == user_to_be_updated.UserId && updated_user.UserName == "username_is_updated").Should().Be(true);
        }
    }
}