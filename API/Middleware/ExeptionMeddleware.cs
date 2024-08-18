using System;
using System.Net;
using System.Text.Json;
using API.Errors;
using Microsoft.AspNetCore.Diagnostics;

namespace API.Middleware;

public class ExeptionMeddleware (RequestDelegate next , ILogger<ExceptionHandlerMiddleware>logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context) 
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex , ex.Message);
            context.Response.ContentType = "appLication/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError ;

            var response = env.IsDevelopment()
            ? new ApiException(context.Response.StatusCode , ex.Message , ex.StackTrace)
            : new ApiException(context.Response.StatusCode , ex.Message , "Intrenal server error");

            var options = new JsonSerializerOptions 
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
                var json = JsonSerializer.Serialize(response , options);

                await context.Response.WriteAsync(json);
        }
    }
}
