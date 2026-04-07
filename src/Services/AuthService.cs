using api_camem.src.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using api_camem.src.Models.Base;
using api_camem.src.Responses;
using api_camem.src.Interfaces;
using api_camem.src.Shared.DTOs;
using api_camem.src.Handlers;
using api_camem.src.Shared.Templates;
using api_camem.src.Shared.Validators;
using api_camem.src.Shared.Utils;
using api_camem.src.Enums.User;

namespace api_camem.src.Services
{
    public class AuthService(IUserRepository userRepository, IProfileUserRepository profileUserRepository, MailHandler mailHandler, MailTemplate mailTemplate, INotificationService notificationService) : IAuthService
    {
        public async Task<ResponseApi<AuthResponse>> LoginAsync(LoginDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email)) return new(null, 400, "E-mail é obrigatório");
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                
                ResponseApi<User?> res = await userRepository.GetByEmailAsync(request.Email);
                if(res.Data is null) return new(null, 400, "Dados incorretos");

                if(res.Data.StatusAccess == "Pendente") return new(null, 400, "Aguarde a aprovação de um Coordenador ou Administrador para acessar o sistema.");
                if(res.Data.StatusAccess == "Reprovado") return new(null, 400, "Acessar ao sistema reprovado.");
                
                User user = res.Data;

                if (!user.ValidatedAccess) {
                    dynamic access = Util.GenerateCodeAccess();
                    
                    res.Data.CodeAccess = access.CodeAccess;
                    res.Data.CodeAccessExpiration = access.CodeAccessExpiration;

                    await userRepository.UpdateAsync(res.Data);

                    await mailHandler.SendMailAsync(request.Email, "Confirmar Conta", await mailTemplate.NewLinkCodeConfirmAccount(res.Data.Name, access.CodeAccess));
                    return new(null, 400, "Conta não confirmada. Verifique seu e-mail.");
                }
                
                if (user.Blocked) return new(null, 400, "Conta bloqueada. Entre em contato com o suporte.");

                bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
                if(!isValid) return new(null, 400, "Dados incorretos");

                AuthResponse response = new ()
                {
                    Token = GenerateJwtToken(user), 
                    RefreshToken = GenerateJwtToken(user, true), 
                    Name = user.Name, 
                    Id = user.Id, 
                    Admin = user.Admin, 
                    Modules = user.Modules, 
                    Photo = user.Photo, 
                    Email = user.Email,
                    Master = user.Master
                };

                return new(response, 200, "Login realizado com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");            
            }
        }

        public async Task<ResponseApi<dynamic>> RegisterAsync(RegisterDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Name)) return new(null, 400, "Nome Completo é obrigatório");
                if (string.IsNullOrEmpty(request.Email)) return new(null, 400, "E-mail é obrigatório");
                if (string.IsNullOrEmpty(request.ProfileUserId)) return new(null, 400, "Vínculo institucional é obrigatório");

                if (string.IsNullOrEmpty(request.Cpf) && request.TypeAccess) return new(null, 400, "CPF é obrigatória");
                if (string.IsNullOrEmpty(request.RA) && !request.TypeAccess) return new(null, 400, "RA é obrigatória");
                
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");

                if (!request.PrivacyPolicy) return new(null, 400, "Aceitar os Termos e Condições e nossa Política de Privacidade é obrigatório");
                
                ResponseApi<User?> isEmail = await userRepository.GetByEmailAsync(request.Email);
                if(isEmail.Data is not null || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                dynamic access = Util.GenerateCodeAccess(5);

                ResponseApi<ProfileUser?> profileUser = await profileUserRepository.GetByIdAsync(request.ProfileUserId);
                RoleEnum role = RoleEnum.Student;
                List<Module> modules = [];
                if(profileUser.Data is not null)
                {
                    role = profileUser.Data.Role;
                    modules = profileUser.Data.Modules;
                }

                User user = new()
                {
                    UserName = $"usuário{access.CodeAccess}",
                    Email = request.Email,
                    Name = request.Name,
                    RA = request.RA,
                    Cpf = request.Cpf,
                    Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    CodeAccess = access.CodeAccess,
                    CodeAccessExpiration = access.CodeAccessExpiration,
                    ValidatedAccess = false,
                    Modules = modules,
                    Admin = false,
                    Master = false,
                    Blocked = false,
                    Role = role,
                    Approved = false,
                    ProfileUserId = request.ProfileUserId,
                    StatusAccess = "Pendente"
                };

                ResponseApi<User?> response = await userRepository.CreateAsync(user);
                if(response.Data is null) return new(null, 400, "Falha ao criar conta.");

                await mailHandler.SendMailAsync(request.Email, "Código de Confirmação", await mailTemplate.ConfirmAccount(request.Name, access.CodeAccess));

                ResponseApi<List<User>> staff = await userRepository.GetUsersNotificatedAsync();
                if(staff.Data is not null)
                {
                    List<string> staffIds = staff.Data.Select(u => u.Id).ToList();

                    if (staffIds.Count > 0)
                    {
                        await notificationService.SendToManyAsync(staffIds, new()
                        {
                            Link      = $"/master-data/users",
                            Title     = "Novo usuário aguardando aprovação",
                            Message   = $"{request.Name} acabou de se cadastrar e aguarda aprovação.",
                            Type      = "Notificacao",
                            CreatedBy = response.Data.Id
                        });
                    }
                }

                return new(null, 201, "Conta criada com sucesso, foi enviado o e-mail de confirmação.");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");            
            }
        }

        public async Task<ResponseApi<dynamic>> ConfirmAccountAsync(ConfirmAccountDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Code)) return new(null, 400, "Código de confirmação é obrigatório");
                
                ResponseApi<User?> user = await userRepository.GetByCodeAccessAsync(request.Code);
                if(user.Data is null) return new(null, 400, "Código inválido.");

                if(user.Data.CodeAccessExpiration < DateTime.UtcNow) return new(null, 400, "Código expirou, solicite um novo código.");

                user.Data.CodeAccess = "";
                user.Data.CodeAccessExpiration = null;
                user.Data.ValidatedAccess = true;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(response.Data is null) return new(null, 400, "Falha ao solicitar novo código.");
                
                return new(null, 200, "Conta verificada com sucesso! Aguarde a aprovação de um Coordenador ou Administrador para acessar o sistema.");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");            
            }
        }

        public async Task<ResponseApi<dynamic>> NewCodeConfirmAsync(NewCodeConfirmDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email)) return new(null, 400, "E-mail é obrigatório");
                
                ResponseApi<User?> user = await userRepository.GetByEmailAsync(request.Email);
                if(user.Data is null || !Validator.IsEmail(request.Email)) return new(null, 400, "E-mail inválido.");

                dynamic access = Util.GenerateCodeAccess(5);

                user.Data.CodeAccess = access.CodeAccess;
                user.Data.CodeAccessExpiration = access.CodeAccessExpiration;

                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(response.Data is null) return new(null, 400, "Falha ao solicitar novo código.");

                await mailHandler.SendMailAsync(request.Email, "Novo Código de Verificação", await mailTemplate.NewCodeConfirmAccount(user.Data.Name, access.CodeAccess));

                return new(null, 200, "Novo código foi enviado.");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");            
            }
        }

        public async Task<ResponseApi<AuthResponse>> RefreshTokenAsync(string token, string planId)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                SecurityToken? validatedToken;

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Environment.GetEnvironmentVariable("ISSUER"),
                    ValidAudience = Environment.GetEnvironmentVariable("AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("SECRET_KEY") ?? "")),
                    ValidateLifetime = false 
                };

                var principal = handler.ValidateToken(token, validationParameters, out validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                if (jwtToken == null) return new(null, 401, "Token inválido.");

                string? tokenType = jwtToken.Claims.FirstOrDefault(c => c.Type == "type")?.Value;
                if (tokenType != "refresh") return new(null, 401, "O token fornecido não é um refresh token.");

                var userId = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub || c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return new(null, 401, "Usuário não encontrado no token.");
                
                var email = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email || c.Type == ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email)) return new(null, 401, "Usuário não encontrado no token.");

                ResponseApi<User?> user = await userRepository.GetByEmailAsync(email);
                if (user.Data is null) return new(null, 401, "Usuário não encontrado.");

                string accessToken = GenerateJwtToken(user.Data);
                string refreshToken = GenerateJwtToken(user.Data, true);

                return new(new AuthResponse
                {
                    Token = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");            
            }
        }

        public async Task<ResponseApi<User>> ResetPasswordAsync(ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha atual é obrigatória");
                if (string.IsNullOrEmpty(request.NewPassword)) return new(null, 400, "Nova senha é obrigatória");
                if (string.IsNullOrEmpty(request.Id)) return new(null, 400, "Falha ao alterar senha");

                if (request.NewPassword != request.ConfirmPassword) return new(null, 400, "Nova senha e confirmação não coincidem");

                if(Validator.IsReliable(request.NewPassword).Equals("Ruim")) return new(null, 400, $"Nova senha é muito fraca");

                ResponseApi<User?> user = await userRepository.GetByIdAsync(request.Id);
                if(!user.IsSuccess || user.Data is null) return new(null, 400, "Falha ao alterar senha");
                
                bool isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Data.Password);
                if(!isValid) return new(null, 400, "Senha atual incorreta");

                user.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                ResponseApi<User?> response = await userRepository.UpdateAsync(user.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao alterar senha");

                return new(null, 200, "Senha alterada com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");            
            }
        }

        public async Task<ResponseApi<User>> RequestForgotPasswordAsync(ForgotPasswordDTO request)
        {
            try
            {
                ResponseApi<User?> responseUser = await userRepository.GetByEmailAsync(request.Email);
                if(responseUser.Data is null) return new(null, 400, "Dados incorretos");

                dynamic access = Util.GenerateCodeAccess();

                responseUser.Data.CodeAccess = access.CodeAccess;
                responseUser.Data.CodeAccessExpiration = access.CodeAccessExpiration;
                responseUser.Data.ValidatedAccess = false;

                string template = await mailTemplate.ForgotPasswordWeb(responseUser.Data.Name, responseUser.Data.CodeAccess);
                await mailHandler.SendMailAsync(request.Email, "Redefinição de Senha", template);

                ResponseApi<User?> response = await userRepository.UpdateAsync(responseUser.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao redefinir senha");

                return new(null, 200, "Foi enviado um e-mail para redefinir sua senha");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");            
            }
        }

        public async Task<ResponseApi<User>> ResetPassordForgotAsync(ResetPasswordDTO request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Password)) return new(null, 400, "Senha é obrigatória");
                if (string.IsNullOrEmpty(request.NewPassword)) return new(null, 400, "Confirmação da senha é obrigatória");
                if (request.Password != request.NewPassword) return new(null, 400, "As senhas não podem ser diferentes");

                ResponseApi<User?> responseUser = await userRepository.GetByCodeAccessAsync(request.CodeAccess);
                if(responseUser.Data is null) return new(null, 400, "Falha ao alterar senha");

                if(responseUser.Data.CodeAccessExpiration < DateTime.UtcNow) return new(null, 400, "Código expirou, solicite um novo e-mail.");
                
                if(Validator.IsReliable(request.Password).Equals("Ruim")) return new(null, 400, $"Senha é muito fraca");

                responseUser.Data.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                responseUser.Data.ValidatedAccess = true;
                responseUser.Data.CodeAccess = "";
                responseUser.Data.CodeAccessExpiration = null;

                ResponseApi<User?> response = await userRepository.UpdateAsync(responseUser.Data);
                if(!response.IsSuccess) return new(null, 400, "Falha ao redefinir senha");
                
                return new(null, 200, "Senha alterada com sucesso");
            }
            catch(Exception ex)
            {
                return new(null, 500, $"Ocorreu um erro inesperado. Por favor, tente novamente mais tarde. {ex.Message}");            
            }
        }

        private static string GenerateJwtToken(User user, bool refresh = false)
        {
            string? SecretKey = Environment.GetEnvironmentVariable("SECRET_KEY") ?? "";
            string? Issuer = Environment.GetEnvironmentVariable("ISSUER") ?? "";
            string? Audience = Environment.GetEnvironmentVariable("AUDIENCE") ?? "";

            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(SecretKey));

            Claim[] claims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Nickname, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("type", refresh ? "refresh" : "access"),
                new Claim("name", user.Name),
                new Claim("role", user.Role.ToString())
            ];

            SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: refresh ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
