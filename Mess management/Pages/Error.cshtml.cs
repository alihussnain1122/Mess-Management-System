using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Diagnostics;

namespace MessManagement.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? ExceptionStackTrace { get; set; }
    public string? ExceptionPath { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    private readonly ILogger<ErrorModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorModel(ILogger<ErrorModel> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        
        if (exceptionHandlerPathFeature != null)
        {
            var exception = exceptionHandlerPathFeature.Error;
            ExceptionPath = exceptionHandlerPathFeature.Path;
            ExceptionMessage = exception.Message;
            ExceptionStackTrace = exception.StackTrace;
            
            // Log the exception with full details
            _logger.LogError(exception, 
                "Unhandled exception occurred. RequestId: {RequestId}, Path: {Path}, Message: {Message}", 
                RequestId, ExceptionPath, exception.Message);
        }
    }
}

