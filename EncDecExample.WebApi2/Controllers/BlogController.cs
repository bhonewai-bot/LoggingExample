using EncDecExample.WebApi2.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EncDecExample.WebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly EncDecService _encDecService;

        public BlogController(EncDecService encDecService)
        {
            _encDecService = encDecService;
        }

        [HttpPost("Login")]
        public IActionResult Login(BlogLoginRequestModel requestModel)
        {
            try
            {
                var result = UserData.Users.FirstOrDefault(x => 
                    x.Username == requestModel.Username && 
                    x.Password == requestModel.Password);

                if (result is null)
                {
                    return Unauthorized("Username or password is incorrect");
                }

                var user = new BlogLoginModel()
                {
                    Username = requestModel.Username,
                    SessionId = Guid.NewGuid().ToString(),
                    SessionExpiry = DateTime.Now.AddHours(1)
                };

                var json = JsonConvert.SerializeObject(user);

                var token = _encDecService.Encrypt(json);

                var model = new BlogLoginResponseModel()
                {
                    AccessToken = token
                };
            
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("UserList")]
        public IActionResult UserList(UserListRequestModel requestModel)
        {
            try
            {
                var json = _encDecService.Decrypt(requestModel.AccessToken);
                var user = JsonConvert.DeserializeObject<BlogLoginModel>(json);
                if (user!.SessionExpiry < DateTime.Now)
                {
                    return Unauthorized("Session expired");
                }
                
                return Ok(UserData.Users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }

    public static class UserData
    {
        public static List<UserDto> Users = new List<UserDto>()
        {
            new UserDto() { Username = "admin", Password = "admin" },
            new UserDto() { Username = "admin2", Password = "admin" },
            new UserDto() { Username = "user", Password = "user" },
        };
    }

    public class BlogLoginRequestModel // Controller
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class BlogLoginResponseModel
    {
        public string AccessToken { get; set; }
    }

    public class BlogLoginModel
    {
        public string Username { get; set; }
        public string SessionId { get; set; }
        public DateTime SessionExpiry { get; set; }
    }

    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserListRequestModel
    {
        public string AccessToken { get; set; }
    }
}