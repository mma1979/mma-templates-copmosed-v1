using System.Text;

namespace WebApplication1.Infrastructure;

public class RequestSanitizationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Check if the request has a body, and it's a text-based content type (you can modify this based on your requirements)
            if (context.Request.HasTextJsonContentType())
            {
                // Read and sanitize the request body
                var originalBodyStream = context.Request.Body;
                try
                {
                    using (var requestBodyStreamReader = new StreamReader(originalBodyStream))
                    {
                        var requestBody = await requestBodyStreamReader.ReadToEndAsync();
                        var sanitizedRequestBody = SanitizePayload(requestBody); // Implement your sanitization logic
                        var sanitizedBytes = Encoding.UTF8.GetBytes(sanitizedRequestBody);
                        context.Request.Body = new MemoryStream(sanitizedBytes);
                    }

                    // Continue processing the request
                    await next(context);
                }
                finally
                {
                    context.Request.Body = originalBodyStream;
                }
            }
            else
            {
                // If the request doesn't need sanitization, continue processing it
                await next(context);
            }
        }

        private string SanitizePayload(string payload)
        {
            // Replace potentially harmful characters with harmless ones
            // This is just a basic example; you should customize this based on your needs

            // Replace script tags
            payload = payload.Replace("<script>", "&lt;script&gt;")
                             .Replace("</script>", "&lt;/script&gt;");

            // Replace potentially harmful SQL characters
            payload = payload.Replace("'", "''");

            // Add more sanitization rules as needed

            return payload;
        }


    }

    public static class HttpRequestExtensions
    {
        // Check if the request content type is JSON
        public static bool HasTextJsonContentType(this HttpRequest request)
        {
            return request.ContentType != null &&
                   request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
        }
    }