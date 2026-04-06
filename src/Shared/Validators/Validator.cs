using System.Text.RegularExpressions;

namespace api_camem.src.Shared.Validators
{

    public static partial class Validator
    {
        [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")]
        private static partial Regex EmailRegex();
        
        [GeneratedRegex("[a-zA-Z]")]
        private static partial Regex LetterRegex();

        [GeneratedRegex("[A-Z]")]
        private static partial Regex UppercaseRegex();

        [GeneratedRegex("[a-z]")]
        private static partial Regex LowercaseRegex();

        [GeneratedRegex(@"\d")]
        private static partial Regex DigitRegex();

        [GeneratedRegex(@"[!@#$%^&*()_+=\-{}\[\]:;""'<>,.?\\/|]")]
        private static partial Regex SpecialRegex();
        
        public static bool IsEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return EmailRegex().IsMatch(email);
        }

        public static string IsReliable(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return "Senha inválida";

            bool hasLetter = LetterRegex().IsMatch(password);
            bool hasUpper = UppercaseRegex().IsMatch(password);
            bool hasLower = LowercaseRegex().IsMatch(password);
            bool hasDigit = DigitRegex().IsMatch(password);
            bool hasSpecial = SpecialRegex().IsMatch(password);
            int length = password.Length;

            if (length < 6 || !(hasLetter && hasDigit)) return "Ruim";

            if (length >= 6 && hasLetter && hasDigit && !hasSpecial) return "Média";

            if (length >= 8 && hasLetter && hasDigit && hasSpecial)
            {
                if (length >= 9 && hasUpper && hasLower) return "Ótima";
                else return "Boa";
            }

            return "Show";
        }
    }
}