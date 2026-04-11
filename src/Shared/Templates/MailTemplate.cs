using api_camem.src.Configuration;
using api_camem.src.Models;
using MongoDB.Driver;

namespace api_camem.src.Shared.Templates
{

    public class MailTemplate(AppDbContext context)
    {
        private static readonly string UiURI =  Environment.GetEnvironmentVariable("UI_URI") ?? "";
        public async Task<string> ForgotPasswordWeb(string name, string code)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "FORGOT_PASSWORD_WEB" && !x.Deleted).FirstOrDefaultAsync();
            string html = code;
            
            if(templates is not null) 
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name).Replace("{{code}}", code).Replace("{{ui_uri}}", UiURI);
            }
            

            return html;
        }
        public async Task<string> FirstAccess(string name)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "FIRST_ACCESS" && !x.Deleted).FirstOrDefaultAsync();
            string html = name;

            if(templates is not null) 
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name).Replace("{{ui_uri}}", UiURI);
            }

            return html;
        }
        public async Task<string> ConfirmAccount(string name, string code)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "CODE_CONFIRM_ACCOUNT" && !x.Deleted).FirstOrDefaultAsync();
            string html = code;
            
            if(templates is not null)
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name).Replace("{{code}}", code);
            }

            return html;
        }
        public async Task<string> NewCodeConfirmAccount(string name, string code)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "NEW_CODE_CONFIRM_ACCOUNT" && !x.Deleted).FirstOrDefaultAsync();
            string html = code;
            
            if(templates is not null)
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name).Replace("{{code}}", code);
            }

            return html;
        }
        public async Task<string> NewLinkCodeConfirmAccount(string name, string code)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "NEW_LINK_CODE_CONFIRM_ACCOUNT" && !x.Deleted).FirstOrDefaultAsync();
            string html = code;
            
            if(templates is not null)
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name).Replace("{{code}}", code).Replace("{{ui_uri}}", UiURI);
            }
            

            return html;
        }
        public async Task<string> NewAccount(string nameAdmin, string name, string email, string createdAt)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "NEW_ACCOUNT" && !x.Deleted).FirstOrDefaultAsync();

            string html = name;
            
            if(templates is not null)
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name)
                .Replace("{{name_admin}}", nameAdmin)
                .Replace("{{email}}", email)
                .Replace("{{createdAt}}", createdAt)
                .Replace("{{ui_uri}}", UiURI);
            }
            

            return html;
        }
        public async Task<string> ApprovedAccount(string name)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "APPROVED_ACCOUNT" && !x.Deleted).FirstOrDefaultAsync();

            string html = name;
            
            if(templates is not null)
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name)
                .Replace("{{ui_uri}}", UiURI);
            }
            

            return html;
        }
        public async Task<string> EventPublish(string name, string nameEvent, string startDate, string endDate, string functions, string hours)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "EVENT_PUBLISHED" && !x.Deleted).FirstOrDefaultAsync();

            string html = name;
            
            if(templates is not null)
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name)
                .Replace("{{name_event}}", nameEvent)
                .Replace("{{functions}}", functions)
                .Replace("{{hours}}", hours)
                .Replace("{{start_date}}", startDate);

                if(!string.IsNullOrEmpty(endDate)) html = html.Replace("{{end_date}}", endDate);
            }
            

            return html;
        }
        public async Task<string> EventFinish(string name, string nameEvent, string startDate, string endDate, string functions, string hours)
        {
            Template? templates = await context.Templates.Find(x => x.Code == "NEW_CERTIFICATE" && !x.Deleted).FirstOrDefaultAsync();

            string html = name;
            
            if(templates is not null)
            {
                html = templates.Html;
                html = html.Replace("{{name}}", name)
                .Replace("{{name_event}}", nameEvent)
                .Replace("{{functions}}", functions)
                .Replace("{{hours}}", hours)
                .Replace("{{start_date}}", startDate);

                if(!string.IsNullOrEmpty(endDate)) html = html.Replace("{{end_date}}", endDate);
            }
            

            return html;
        }
    }
}