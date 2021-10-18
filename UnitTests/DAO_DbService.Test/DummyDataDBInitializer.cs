using DAO_DbService.Contexts;
using DAO_DbService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAO_DbService.Test
{
    public class DummyDataDBInitializer
    {
        public DummyDataDBInitializer()
        {

        }
        public void Seed(dao_maindb_context context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            context.Users.AddRange(
                new User() {
                    NameSurname = "John Doe",
                    Email = "j.doe@test.com",
                    Password = "password",
                    Newsletter = false,
                    UserType = "",
                    IsBlocked = false,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    KYCStatus = true,
                    FailedLoginCount = 0,
                    ProfileImage = "",
                    UserName = "J.Doe@test"
                },
                new User()
                {
                    NameSurname = "John Doe",
                    Email = "j.doe@test.com",
                    Password = "password",
                    Newsletter = false,
                    UserType = "",
                    IsBlocked = false,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    KYCStatus = true,
                    FailedLoginCount = 0,
                    ProfileImage = "",
                    UserName = "J.Doe@test"
                },
                new User()
                {
                    NameSurname = "John Doe",
                    Email = "j.doe@test.com",
                    Password = "password",
                    Newsletter = false,
                    UserType = "",
                    IsBlocked = false,
                    IsActive = true,
                    CreateDate = DateTime.Now,
                    KYCStatus = true,
                    FailedLoginCount = 0,
                    ProfileImage = "",
                    UserName = "J.Doe@test"
                });
            context.SaveChanges();


        }

    }
}
