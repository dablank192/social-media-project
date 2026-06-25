using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Exception.ImageException;
using vsa_w_controller_csharp.Feature.Blog.UpdateAUserBlog;

namespace vsa_w_controller_csharp.Infrastructure;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment env
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        System.Exception exception,
        CancellationToken ct
    )
    {
        logger.LogError(exception, exception.Message);

        var problemDetail = new ProblemDetails
        {
            Title = "Error",
            Detail= "An unknown error has occur",
            Status= StatusCodes.Status500InternalServerError,
            Instance= context.Request.Path
        };

        if(exception is DuplicateUsernameException)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            problemDetail.Detail = "User is already exist";
            problemDetail.Status = StatusCodes.Status409Conflict;
        }

        else if (exception is InvalidCredentialsException)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            problemDetail.Detail = "Invalid username or password";
            problemDetail.Status = StatusCodes.Status401Unauthorized;
        }

        else if (exception is InvalidRefreshTokenException)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            problemDetail.Detail = "Can't identify refresh token";
            problemDetail.Status = StatusCodes.Status401Unauthorized;
        }

        else if (exception is DeleteBlogNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            problemDetail.Detail = "Can't find blog's id for termination";
            problemDetail.Status = StatusCodes.Status404NotFound;
        }

        else if (exception is UpdateBlogNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status200OK;
            problemDetail.Detail = "Can't find blog's id for update";
            problemDetail.Status = StatusCodes.Status404NotFound;
        }

        else if (exception is UploadImageException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            problemDetail.Detail = "Exception while uploading image";
            problemDetail.Status = StatusCodes.Status400BadRequest;
        }

        if (env.IsDevelopment())
        {
            problemDetail.Extensions.Add("Detail", exception.Message);
            problemDetail.Extensions.Add("Traceback", exception.StackTrace);
        }

        await context.Response.WriteAsJsonAsync(problemDetail, ct);

        return true;
    }
}
