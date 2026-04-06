using api_camem.src.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using SixLabors.ImageSharp;

namespace api_camem.src.Handlers
{
    public class UploadHandler(Cloudinary cloudinary, ILoggerService loggerService)
    {
        public async Task<string> UploadAttachment(string parent, IFormFile attachment, string apiPath = "")
        {
            try
            {
                
                string extension = Path.GetExtension(attachment.FileName).ToLower();
                bool isHeic = extension == ".heic" || extension == ".heif";
                string fileName = Guid.NewGuid().ToString();

                using var memoryStream = new MemoryStream();

                if (isHeic)
                {
                    using var inputStream = attachment.OpenReadStream();
                    using var image = await Image.LoadAsync(inputStream);
                    await image.SaveAsJpegAsync(memoryStream);
                    memoryStream.Position = 0;
                    extension = ".jpg";
                }
                else
                {
                    await attachment.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                }

                var uploadParams = new RawUploadParams
                {
                    File = new FileDescription(fileName + extension, memoryStream),
                    Folder = $"projeto-modelo/{parent}",
                    PublicId = fileName
                };

                var result = await cloudinary.UploadAsync(uploadParams);

                if (result.Error != null) 
                {
                    await loggerService.CreateAsync(new()
                    {
                        Method = "UPLOAD_ATTACHMENT",
                        Path = apiPath,
                        Message = $"Failed to upload attachment: {result.Error.Message}",
                        StatusCode = 500
                    });
                    return "";
                } 

                return result.SecureUrl.ToString();
            }
            catch (Exception ex)
            {
                await loggerService.CreateAsync(new()
                {
                    Method = "UPLOAD_ATTACHMENT",
                    Path = apiPath,
                    Message = $"Failed to upload attachment: {ex.Message}",
                    StatusCode = 500
                });
                return "";
            }
        }
    }
}