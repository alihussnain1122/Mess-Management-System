using Microsoft.AspNetCore.Mvc;

namespace MessManagement.Helpers
{
    /// <summary>
    /// Helper class for AJAX responses to prevent page reloads
    /// </summary>
    public static class AjaxResponse
    {
        /// <summary>
        /// Returns a success JSON result
        /// </summary>
        public static JsonResult Success(string message = "Operation completed successfully", object? data = null, string? redirectUrl = null)
        {
            return new JsonResult(new
            {
                success = true,
                message,
                data,
                redirectUrl,
                refresh = redirectUrl == null
            });
        }

        /// <summary>
        /// Returns an error JSON result
        /// </summary>
        public static JsonResult Error(string message = "An error occurred", object? errors = null)
        {
            return new JsonResult(new
            {
                success = false,
                message,
                errors
            })
            {
                StatusCode = 400
            };
        }

        /// <summary>
        /// Returns a validation error JSON result
        /// </summary>
        public static JsonResult ValidationError(Dictionary<string, string[]> errors)
        {
            return new JsonResult(new
            {
                success = false,
                message = "Please correct the validation errors",
                errors
            })
            {
                StatusCode = 400
            };
        }

        /// <summary>
        /// Check if the request is an AJAX request
        /// </summary>
        public static bool IsAjaxRequest(HttpRequest request)
        {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }
    }
}
