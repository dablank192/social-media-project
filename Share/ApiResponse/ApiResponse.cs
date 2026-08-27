using System;
using Microsoft.AspNetCore.StaticFiles;

namespace vsa_w_controller_csharp.Share.ApiResponse;

public record ApiResponse<T>(
    string? Error,
    string? Message,
    T? Response
)

{
    public static ApiResponse<T> Success( //created push/update data response template for POST/PATCH API
        string? SuccessMessage,
        T SuccessResponse
    )
    {
        var response = new ApiResponse<T>(
            Error: null,
            Message: SuccessMessage,
            Response: SuccessResponse
        );

        return response;
    }
}
