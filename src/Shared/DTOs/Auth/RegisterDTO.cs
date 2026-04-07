using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using api_camem.src.Enums.User;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace api_camem.src.Shared.DTOs
{
    public class RegisterDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RA { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool PrivacyPolicy { get; set; } = false;
        public bool TypeAccess { get; set; } = true;
        public string ProfileUserId { get; set; } = string.Empty;
    }
}