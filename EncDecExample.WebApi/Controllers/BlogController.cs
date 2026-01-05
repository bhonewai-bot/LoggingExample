using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EncDecExample.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace EncDecExample.WebApi.Controllers
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
        public IActionResult Login(BlogLoginRequestModel request)
        {
            try
            {
                var result = UserData.Users.FirstOrDefault(x => 
                    x.Username == request.Username && 
                    x.Password == request.Password);

                if (result is null)
                {
                    return Unauthorized();
                }

                var user = new BlogLoginModel()
                {
                    SessionExpiry = DateTime.Now.AddMinutes(10),
                    SessionId = Guid.NewGuid().ToString(),
                    Username = result.Username,
                };
                
                var json = JsonConvert.SerializeObject(user);

                var token = _encDecService.Encrypt(json);
                var model = new BlogLoginResponseModel()
                {
                    AccessToken = token,
                };
                return Ok(model);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpPost("UserList")]
        public IActionResult UserList(UserListRequestModel request)
        {
            try
            {
                var result = HttpContext.Request.Headers.TryGetValue("Authorization", out var accessToken);
                if (!result)
                {
                    return Unauthorized("Access token is required.");
                }
                var json = _encDecService.Decrypt(accessToken.ToString());
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
        
        [ServiceFilter(typeof(ValidationTokenActionFilter))]
        [HttpPost("UserList2")]
        public IActionResult UserList2(UserListRequestModel request)
        {
            try
            {
                
                /*var json = _encDecService.Decrypt(accessToken.ToString());
                var user = JsonConvert.DeserializeObject<BlogLoginModel>(json);
                if (user!.SessionExpiry < DateTime.Now)
                {
                    return Unauthorized("Session expired");
                }*/
                return Ok(UserData.Users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }

    public class UserListRequestModel
    {
        public string? AccessToken { get; set; }
    }

    public static class UserData
    {
        public static List<UserDto> Users = new List<UserDto>()
        {
            new UserDto() { Username = "admin", Password = "sasa@123" },
            new UserDto() { Username = "admin2", Password = "sasa@123" },
            new UserDto() { Username = "user", Password = "sasa@123" },
        };
    }

    public class BlogLoginRequestModel // Controller > RequestModel
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

    public class UserDto // Database Mapping
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
    public class ValidationTokenActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(
            ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Do something before the action executes.
            var result = context.HttpContext.Request.Headers.TryGetValue("Authorization", 
                out var accessToken);
            if (!result)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var encDecService = context.HttpContext.RequestServices.GetRequiredService<EncDecService>();
            
            var json = encDecService.Decrypt(accessToken.ToString());
            var user = JsonConvert.DeserializeObject<BlogLoginModel>(json);
            if (user!.SessionExpiry < DateTime.Now.AddMinutes(-10))
            {
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
            // Do something after the action executes.
        }
    }
    
    public class ValidationTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        } 

        public async Task InvokeAsync(HttpContext context)
        {
            /*if (context.Request.Path.ToString().ToLower() == "/weatherforecast")
            {
                goto Result;
            }*/
            var requestPath = context.Request.Path.ToString().ToLower();
            if (AllowUrlList.Contains(requestPath))
            {
                goto Result;
            }
            
            // Do something before the action executes.
            var result = context.Request.Headers.TryGetValue("Authorization", 
                out var accessToken);

            if (!result)
            {
                context.Response.StatusCode = 401;
                return;
            }

            var encDecService = context.RequestServices.GetRequiredService<EncDecService>();
            var json = encDecService.Decrypt(accessToken.ToString());
            var user = JsonConvert.DeserializeObject<BlogLoginModel>(json);
            if (user!.SessionExpiry < DateTime.Now.AddMinutes(-10))
            {
                context.Response.StatusCode = 401;
                return;
            }
            
            Result:
            await _next(context);
            // Do something after the action executes.
        }

        private string[] AllowUrlList =
        {
            "/weatherforecast",
            "/api/blog/login"
        };
    }
    
    public static class RequestCultureMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidateTokenMiddleware(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<ValidationTokenMiddleware>();
        }
    }
}
