using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using System.Text.RegularExpressions;

namespace api_camem.src.Handlers
{
    public class SmsHandler
    {
        private readonly string fromNumber = "";

        public SmsHandler()
        {
            string? AccountSidPhone = Environment.GetEnvironmentVariable("ACCOUNT_SID_PHONE") ?? "";
            string? AuthTokenPhone = Environment.GetEnvironmentVariable("AUTH_TOKEN_PHONE") ?? "";
            fromNumber = Environment.GetEnvironmentVariable("FROM_PHONE") ?? "";

            TwilioClient.Init(AccountSidPhone, AuthTokenPhone);
        }

        public async Task<bool> SendMessageAsync(string toPhone, string message)
        {
            try
            {
                string phoneFormeted = Regex.Replace(toPhone, @"[^\d]", "");
                await MessageResource.CreateAsync(
                    to: new PhoneNumber($"+55{phoneFormeted}"),
                    from: new PhoneNumber(fromNumber),
                    body: $"{message}"
                );

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}