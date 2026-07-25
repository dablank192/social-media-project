using System;

namespace vsa_w_controller_csharp.Share.CloudinaryImgUpload;

public interface ICldImageManagement
{
    public Task<ImageMetadata> UploadImageToCldAsync(IFormFile fileImage, string folderName);
    public Task DeleteImageCldAsync(string publicId);
}
