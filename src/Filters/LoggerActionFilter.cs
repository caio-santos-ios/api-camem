using api_camem.src.Interfaces;
using api_camem.src.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace api_camem.src.Filters
{
    public class LoggerActionFilter(ILoggerService loggerService) : IAsyncActionFilter
    {
        private static readonly HashSet<string> IgnoredPaths =
        [
            "/api/loggers",
            "/api/check",
            "/api/auth/login",
            "/api/auth/register",
            "/api/notifications/send"
        ];
        private static readonly HashSet<string> AuditPaths =
        [
            "/api/customers",
            "/api/suppliers",
        ];

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            
            ActionExecutedContext executed = await next();

            sw.Stop();

            string path = context.HttpContext.Request.Path.Value?.ToLower() ?? "";

            if (IgnoredPaths.Any(p => path.StartsWith(p))) return;

            int statusCode = executed.Result switch
            {
                ObjectResult obj    => obj.StatusCode ?? 200,
                StatusCodeResult sc => sc.StatusCode,
                _                   => context.HttpContext.Response.StatusCode
            };
            
            string message = ExtractMessage(executed.Result) ?? ResolveDefaultMessage(statusCode);
            
            string? userId = context.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            string method = context.HttpContext.Request.Method.ToUpper();
            bool isAudit  = false;

            if(AuditPaths.Contains(path))
            {
                isAudit = true;
            }

            if (executed.Exception is not null && !executed.ExceptionHandled)
            {
                message    = executed.Exception.Message;
                statusCode = 500;
            }

            await loggerService.CreateAsync(new CreateLoggerDTO
            {
                Path       = context.HttpContext.Request.Path,
                Method     = method,
                Message    = message,
                StatusCode = statusCode,
                CreatedBy  = userId ?? string.Empty,
                Time       = Math.Round(sw.Elapsed.TotalSeconds, 3),
                Audit = isAudit
            });
        }

        private static string? ExtractMessage(IActionResult? result)
        {
            if (result is not ObjectResult obj || obj.Value is null) return null;

            var value = obj.Value;
            var type  = value.GetType();

            var messageProp = type.GetProperty("Message") ?? type.GetProperty("message");

            if (messageProp is not null) return messageProp.GetValue(value)?.ToString();

            var resultProp = type.GetProperty("Result") ?? type.GetProperty("result");

            if (resultProp is not null)
            {
                var inner      = resultProp.GetValue(value);
                var innerMsg   = inner?.GetType().GetProperty("Message") ?? inner?.GetType().GetProperty("message");
                if (innerMsg is not null) return innerMsg.GetValue(inner)?.ToString();
            }

            return null;
        }

        private static string ResolveDefaultMessage(int statusCode) => statusCode switch
        {
            200 => "OK",
            201 => "Criado com sucesso",
            204 => "Excluído com sucesso",
            400 => "Requisição inválida",
            401 => "Não autorizado",
            403 => "Acesso negado",
            404 => "Não encontrado",
            500 => "Erro interno",
            _   => $"Status {statusCode}"
        };
    }
}