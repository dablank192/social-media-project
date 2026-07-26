using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using vsa_w_controller_csharp.Exception.ImageException;

namespace vsa_w_controller_csharp.Share.CloudinaryImgUpload;

public record ImageMetadata(
    string? SecureUrl,
    string? PublicId
);

public class CldImageManagement(
    Cloudinary cloudinaryClient
) : ICldImageManagement
{
    public async Task<ImageMetadata> UploadImageToCldAsync(IFormFile fileImage, string folderName)
    {
        var allowedExtension = new[] {".jpeg", ".png", ".jpg", ".webp"};

        var fileExtension = Path.GetExtension(fileImage.FileName).ToLowerInvariant();

        var fileMaxSize = 5 * 1024 * 1024; //5MB

        if(fileImage == null || fileImage.Length == 0)
        {
            throw new CannotFindImageException();
        }
        else if (fileImage.Length > fileMaxSize)
        {
            throw new FileTooLargeException();
        }
        else if (!allowedExtension.Contains(fileExtension))
        {
            throw new InvalidFileFormat();
        }

        try
        {
            using var fileStream = fileImage.OpenReadStream();

            var file = new ImageUploadParams
            {
                File =  new FileDescription(fileImage.FileName, fileStream),
                Folder = $"Image/{folderName}"
            };

            var result = await cloudinaryClient.UploadAsync(file);
            var url = result.SecureUrl.ToString();
            var publicId = result.PublicId;

            var response = new ImageMetadata(
                SecureUrl: url,
                PublicId: publicId
            );

            return response;
        }
        catch (System.Exception ex)
        {
            throw new UploadImageException(ex.ToString());
        }

        //chua test
    }

    public async Task DeleteImageCldAsync(string publicId)
    {
        try
        {
            var deletedImg = new DeletionParams(publicId: publicId);
            
            var result = await cloudinaryClient.DestroyAsync(deletedImg);
            if(result.Error != null || result.Result != "ok")
            {
                throw new DeleteImageException(result.Error.Message);
            }
        }
        catch(System.Exception ex)
        {
            throw new DeleteImageException(ex.ToString());
        }
        
        //chua test
    }
}
