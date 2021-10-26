using System;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Helpers.Models.DtoModels.MainDbDto;

namespace DAO_DbService.Test
{
    public class UsersControllerTest
    {
        PostUnitTestController controller = new PostUnitTestController();
        [Fact]
        public void getUsersListTest()
        {
            var userList = controller.usersControllers.Get();

            userList.Should().HaveCount(3); //PASSED  
        }

        [Fact]
        public void UserSearchTestwithName()
        {
            IEnumerable<UserDto> foundUser;

            foundUser = controller.usersControllers.UserSearch("John Doe");

            foundUser.Should().HaveCount(3);

        }  

        [Fact]
        public void UserSearchTestwithEmail()
        {
            IEnumerable<UserDto> foundUser;

            foundUser = controller.usersControllers.UserSearch("j.doe@test.com");

            foundUser.Should().HaveCount(3);
        }     
    }
}
