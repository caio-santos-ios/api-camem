using api_camem.src.Handlers;
using api_camem.src.Interfaces;
using api_camem.src.Models;
using api_camem.src.Models.Base;
using api_camem.src.Shared.DTOs;
using api_camem.src.Shared.DTOs.CustomCertificate;
using api_camem.src.Shared.Utils;
using AutoMapper;
using Docnet.Core;
using Docnet.Core.Models;
using SkiaSharp;

namespace api_camem.src.Services
{
    public class CustomCertificateService(ICustomCertificateRepository repository, IMapper _mapper, UploadHandler uploadHandler, IEventRepository eventRepository, IEventParticipantRepository eventParticipantRepository, IEventParticipantFunctionRepository eventParticipantFunctionRepository, ICertificateRepository certificateRepository) : ICustomCertificateService
    {
        #region READ
        public async Task<ResponseApi<PaginationApi<List<dynamic>>>> GetAllAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<CustomCertificate> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> certificates = await repository.GetAllAsync(pagination);
                int count = await repository.GetCountDocumentsAsync(pagination);
                PaginationApi<List<dynamic>> data = new(certificates.Data, count, pagination.PageNumber, pagination.PageSize);
                return new(data, 200, "Certificados listados com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetByIdAggregateAsync(string id)
        {
            try
            {
                ResponseApi<dynamic?> certificate = await repository.GetByIdAggregateAsync(id);
                if (certificate.Data is null) return new(null, 404, "Certificado não encontrada");
                return new(certificate.Data, 200, "Certificado obtido com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetLastAsync()
        {
            try
            {
                ResponseApi<dynamic?> certificate = await repository.GetByIdAggregateAsync("");
                if (certificate.Data is null) return new(null, 404, "Certificado não encontrada");
                return new(certificate.Data, 200, "Certificado obtido com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<dynamic?>> GetValidateKeyAsync(string keyCustomCertificate)
        {
            try
            {
                ResponseApi<dynamic?> certificate = await repository.GetValidateKeyAsync(keyCustomCertificate);
                if (certificate.Data is null) return new(null, 404, "Certificado não encontrada");
                return new(certificate.Data, 200, "Certificado obtido com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<List<dynamic>>> GetSelectAsync(GetAllDTO request)
        {
            try
            {
                PaginationUtil<CustomCertificate> pagination = new(request.QueryParams);
                ResponseApi<List<dynamic>> certificates = await repository.GetSelectAsync(pagination);
                return new(certificates.Data, 200, "Certificados listados com sucesso");
            }
            catch
            {
                return new(null, 500, "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
        #endregion

        #region CREATE
        public async Task<ResponseApi<CustomCertificate?>> CreateAsync(CreateCustomCertificateDTO request)
        {
            try
            {
                CustomCertificate certificate = _mapper.Map<CustomCertificate>(request);

                ResponseApi<CustomCertificate?> response = await repository.CreateAsync(certificate);

                if (response.Data is null) return new(null, 400, "Falha ao criar Certificado.");
                return new(response.Data, 201, "Certificado criado com sucesso.");
            }
            catch
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        public async Task<ResponseApi<CustomCertificate?>> CreateCertificateManualAsync(CreateCertificateManualDTO request)
        {
            try
            {
                List<CreateCertificateManualParticipantFunction> createCertificateManualParticipantFunctions = [];

                var list = request.ParticipantFunction.Split(";");
                foreach (var item in list)
                {
                    var function = item.Split(",");
                    createCertificateManualParticipantFunctions.Add(new()
                    {
                        Name = function[0],
                        Hours = Convert.ToInt32(function[1])
                    });
                }

                Event evenT = new()
                {
                    Title = request.NameEvent,
                    Description = request.NameEvent,
                    StartDate = request.StartDateEvent,
                    EndDate = request.EndDateEvent,
                    Status = "Finalizado",
                    KeyCertificate = request.KeyCertificateEvent,
                    RegisterBookNumber = request.RegisterBookNumberEvent,
                    RegisterFolderNumber = request.RegisterFolderNumberEvent,
                    CertificateId = "",
                    UserIds = [],
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.Now
                };

                await eventRepository.CreateAsync(evenT);

                EventParticipant eventParticipant = new()
                {
                    Name = request.NameParticipantEvent,
                    Hours = createCertificateManualParticipantFunctions.Sum(x => x.Hours),
                    UserId = request.UserId,
                    EventId = evenT.Id,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.Now
                };

                await eventParticipantRepository.CreateAsync(eventParticipant);

                foreach (CreateCertificateManualParticipantFunction participantFunction in createCertificateManualParticipantFunctions)
                {
                    EventParticipantFunction eventParticipantFunction = new()
                    {
                        EventId = evenT.Id,
                        EventParticipantId = eventParticipant.Id,
                        Name = participantFunction.Name,
                        Hours = participantFunction.Hours,
                        IsPresence = true,
                        CreatedBy = request.CreatedBy,
                        CreatedAt = DateTime.Now
                    };

                    await eventParticipantFunctionRepository.CreateAsync(eventParticipantFunction);
                }

                string uriCertificate = await uploadHandler.UploadAttachment("certificate-manual", request.Certificate!, "/api/certificate-manual");

                Certificate certificate = new()
                {
                    Origin = "manual",
                    Name = request.NameEvent,
                    EventId = evenT.Id,
                    UserId = request.UserId,
                    Hours = createCertificateManualParticipantFunctions.Sum(x => x.Hours),
                    KeyCertificate = request.KeyCertificateEvent,
                    Html = uriCertificate
                };

                await certificateRepository.CreateAsync(certificate);

                return new(null, 201, "Certificado manual criado com sucesso.");
            }
            catch
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde");
            }
        }
        #endregion

        #region UPDATE
        public async Task<ResponseApi<CustomCertificate?>> UpdateAsync(UpdateCustomCertificateDTO request)
        {
            try
            {
                ResponseApi<CustomCertificate?> certificateResponse = await repository.GetByIdAsync(request.Id);
                if (certificateResponse.Data is null) return new(null, 404, "Falha ao atualizar");

                CustomCertificate certificate = _mapper.Map<CustomCertificate>(request);
                certificate.UpdatedAt = DateTime.UtcNow;
                certificate.CreatedAt = certificateResponse.Data.CreatedAt;

                ResponseApi<CustomCertificate?> response = await repository.UpdateAsync(certificate);
                if (!response.IsSuccess) return new(null, 400, "Falha ao atualizar");

                return new(response.Data, 200, "Atualizada com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        public async Task<ResponseApi<string?>> UploadSignature(UploadSignatureCustomCertificateDTO request)
        {
            try
            {
                using var pdfStream = new MemoryStream();
                await request.Photo.CopyToAsync(pdfStream);
                var pdfBytes = pdfStream.ToArray();

                using var docReader = DocLib.Instance.GetDocReader(pdfBytes, new PageDimensions(1080, 1920));
                using var pageReader = docReader.GetPageReader(0);

                var rawBytes = pageReader.GetImage();
                int width = pageReader.GetPageWidth();
                int height = pageReader.GetPageHeight();

                var info = new SKImageInfo(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
                var bitmap = new SKBitmap();
                var handle = System.Runtime.InteropServices.GCHandle.Alloc(rawBytes, System.Runtime.InteropServices.GCHandleType.Pinned);
                bitmap.InstallPixels(info, handle.AddrOfPinnedObject());
                handle.Free();

                using var image = SKImage.FromBitmap(bitmap);
                using var data = image.Encode(SKEncodedImageFormat.Png, 100);
                var pngBytes = data.ToArray();

                using var ms = new MemoryStream(pngBytes);
                var pngFile = new FormFile(ms, 0, ms.Length, "photo", "signature.png")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/png"
                };

                string uriPhoto = await uploadHandler.UploadAttachment("signatures", pngFile, "/api/signature");

                return new(uriPhoto, 200, "Atualizada com sucesso");
            }
            // catch (Exception ex)
            // {
            //     return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            // }
            catch (Exception ex)
            {
                var inner = ex;
                while (inner.InnerException != null) inner = inner.InnerException;
                return new(null, 500, $"Erro: {ex.Message} | Causa raiz: {inner.GetType().Name}: {inner.Message}");
            }
        }

        #endregion

        #region DELETE
        public async Task<ResponseApi<CustomCertificate>> DeleteAsync(string id)
        {
            try
            {
                ResponseApi<CustomCertificate> certificate = await repository.DeleteAsync(id);
                if (!certificate.IsSuccess) return new(null, 400, certificate.Message);
                return new(null, 204, "Excluída com sucesso");
            }
            catch (Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");
            }
        }
        #endregion 
    }
}