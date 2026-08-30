using System;

namespace vsa_w_controller_csharp.Share.ApiResponse;

public record PageResponse<T>(
    List<T> Data,
    int? PageIndex,
    int? PageSize,
    int? TotalRecord,
    int? TotalPage
); //response template cho API có sử dụng phân trang
// Khi sử dụng template phân trang này, lấy dữ liệu kết quả của API Handle, sau đó gán dữ liệu đó vào type của template này, và gán template này vào repsonse template cha trước khi trả về cho Client thông qua return Ok()
// Chưa test