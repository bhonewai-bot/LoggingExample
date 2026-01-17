using EncDecExample.WebApi3.Controllers;
using EncDecExample.WebApi3.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EncDecExample.WebApi3.Middlewares;

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
        var result = context.Request.Headers.TryGetValue("Authorization", out var accessToken);
        if (!result)
        {
            context.Response.StatusCode = 401;
            return;
        }

        var encDecService = context.RequestServices.GetRequiredService<EncDecService>();
        var json = encDecService.Decrypt(accessToken!);
        var user = JsonConvert.DeserializeObject<UserRequestModel>(json);
        if (user!.SessionExpiry < DateTime.Now)
        {
            context.Response.StatusCode = 403;
            return;
        }

        Result:
        // Call the next delegate/middleware in the pipeline.
        await _next(context);
    }

    private string[] AllowUrlList =
    {
        "/weatherforecast",
        "/api/user/login"
    };
}

public static class ValidationTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseValidateTokenMiddleware(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<ValidationTokenMiddleware>();
    }
}