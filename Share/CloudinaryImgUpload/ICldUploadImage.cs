using System;

namespace vsa_w_controller_csharp.Share.CloudinaryImgUpload;

public interface ICldUploadImage
{
    public Task<ImageMetadata> UploadImageToCldAsync(IFormFile fileImage);
}
