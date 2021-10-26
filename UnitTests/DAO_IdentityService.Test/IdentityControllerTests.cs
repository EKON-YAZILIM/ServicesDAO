using FluentAssertions;
using Helpers.Models.DtoModels.MainDbDto;
using Helpers.Models.IdentityModels;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DAO_IdentityService.Test
{
    public class IdentityControllerTests: IClassFixture<WebApplicationFactory<DAO_IdentityService.Startup>>
    {
        readonly HttpClient _client;

        public IdentityControllerTests(WebApplicationFactory<DAO_IdentityService.Startup> fixture)
        {
            _client = fixture.CreateClient();
        }

        [Fact]
        public async Task Login()
        {
            string userJson = string.Empty;

            LoginModel user1 = new LoginModel() { 
                email ="",
                pass = "",
                application = Helpers.Constants.Enums.AppNames.DAO_ApiGateway,
                ip = "",
                port =""
            };

            //_client.BaseAddress = new Uri(Program._settings.Service_Db_Url + "/users/GetByEmail?email=" + user1.email);
            var r1 = Helpers.Request.Get("https://localhost:9990/users/get");
            List<UserDto> r2 = Helpers.Serializers.DeserializeJson<List<UserDto>>(r1);


            //var response1 = await _client.GetAsync("https://localhost:9990/users/Gets");
            //response1.StatusCode.Should().Be(HttpStatusCode.OK);
            
            userJson = Helpers.Request.Get(Program._settings.Service_Db_Url + "/users/GetByEmail?email=" + user1.email);

            r2.GetType().Should().NotBe(null);




        }
    }
}
