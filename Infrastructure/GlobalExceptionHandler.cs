using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using vsa_w_controller_csharp.Exception.AuthException;
using vsa_w_controller_csharp.Exception.BlogException;
using vsa_w_controller_csharp.Exception.Comment;
using vsa_w_controller_csharp.Exception.ImageException;
using vsa_w_controller_csharp.Exception.UserException;
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


        var (statusCode, detailMessage) = exception switch
        {
            DuplicateUsernameException => (
                StatusCodes.Status409Conflict,
                "User is already exist"
            ),

            InvalidCredentialsException => (
                StatusCodes.Status401Unauthorized,
                "Invalid username or password"
            ),

            DeleteBlogNotFoundException => (
                StatusCodes.Status404NotFound,
                "Can't find blog's id for termination"
            ),

            InvalidRefreshTokenException => (
                StatusCodes.Status401Unauthorized,
                "Can't identify refresh token"
            ),

            UploadImageException => (
                StatusCodes.Status400BadRequest,
                "Exception while uploading image"
            ),

            UpdateBlogNotFoundException => (
                StatusCodes.Status404NotFound,
                "Can't find blog's id for update"
            ),

            CommentNotFoundException => (
                StatusCodes.Status404NotFound,
                "Can't find parent comment's id"
            ),

            UserIdNotFoundException => (
                StatusCodes.Status404NotFound,
                "Can't find User's Id"
            ),

            UserProfileNotFoundException => (
                StatusCodes.Status404NotFound,
                "User's profile was either not existed or was set private"
            ),

            InvalidUserProfileStatusException => (
                StatusCodes.Status404NotFound,
                "Profile's status are the same as the requested status"
            ),
            CannotFindImageException => (
                StatusCodes.Status404NotFound,
                "System can't recognize image file"
            ),
            FileTooLargeException => (
                StatusCodes.Status406NotAcceptable,
                "The upload file is too large to process"
            ),
            InvalidFileFormat => (
                StatusCodes.Status400BadRequest,
                "File format is not valid"
            ),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unknown error has occur"
            )
        };

        context.Response.StatusCode = statusCode;

        var problemDetail = new ProblemDetails
        {
            Detail = detailMessage,
            Status = statusCode,
            Instance = context.Request.Path
        };


        if (env.IsDevelopment())
        {
            problemDetail.Extensions.Add("SystemDetail", exception.Message);
            problemDetail.Extensions.Add("Traceback", exception.StackTrace);
        }

        await context.Response.WriteAsJsonAsync(problemDetail, ct);

        return true;
    }
}
