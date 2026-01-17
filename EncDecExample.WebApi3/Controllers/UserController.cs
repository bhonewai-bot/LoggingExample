using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EncDecExample.WebApi3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace EncDecExample.WebApi3.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly EncDecService _encDecService;

    public UserController(EncDecService encDecService)
    {
        _encDecService = encDecService;
    }

    [HttpPost("Login")]
    public IActionResult Login(UserLoginRequestModel requestModel)
    {
        try
        {
            var user = UserData.Users.FirstOrDefault(x =>
                x.Username == requestModel.Username && x.Password == requestModel.Password);

            if (user is null)
            {
                return Unauthorized("Invalid username or password.");
            }

            var result = new UserRequestModel()
            {
                Username = requestModel.Username,
                SessionId = Guid.NewGuid().ToString(),
                SessionExpiry = DateTime.Now.AddMinutes(1)
            };
            
            var json = JsonConvert.SerializeObject(result);

            var token = _encDecService.Encrypt(json);

            var model = new UserLoginResponseModel()
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
    public IActionResult UserList(UserListRequestModel requestModel)
    {
        try
        {
            var result = HttpContext.Request.Headers.TryGetValue("Authorization", out var accessToken);
            if (!result)
            {
                return Unauthorized("Access token is required.");
            }
            
            var token = _encDecService.Decrypt(accessToken.ToString());
            var user = JsonConvert.DeserializeObject<UserRequestModel>(token);
            if (user!.SessionExpiry < DateTime.Now)
            {
                return Unauthorized("Session expired.");
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
    public IActionResult UserList2(UserListRequestModel requestModel)
    {
        try
        {
            /*var token = _encDecService.Decrypt(accessToken.ToString());
            var user = JsonConvert.DeserializeObject<UserRequestModel>(token);
            if (user!.SessionExpiry < DateTime.Now)
            {
                return Unauthorized("Session expired.");
            }*/

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

public class UserLoginRequestModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserLoginResponseModel
{
    public string AccessToken { get; set; }
}

public class UserRequestModel
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
    public string? AccessToken { get; set; }
}

public class ValidationTokenActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Do something before the action executes.
        var result = context.HttpContext.Request.Headers.TryGetValue("Authorization", out var accessToken);
        if (!result)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var encDecService = context.HttpContext.RequestServices.GetRequiredService<EncDecService>();
        var json = encDecService.Decrypt(accessToken!);
        var user = JsonConvert.DeserializeObject<UserRequestModel>(json);
        if (user!.SessionExpiry < DateTime.Now)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        await next();
        // Do something after the action executes.
    }
}