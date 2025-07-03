
# Json to Columnar Compression 


- Efficiently converts JSON to columnar format and back, enabling faster transmission and reduced storage. 
- This approach is especially beneficial when working with large datasets .
- You can provide JSON in any structure or format.


## Usage

```c#


// Example on a Middleware




using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using json_columnar_compression;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Text.Json.Serialization;

namespace Cp.Api.CustomClasses.ControllerFilters
{


    public class RequestResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Enable buffering so we can read the body multiple times
            context.Request.EnableBuffering();

            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            string body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    // apply message pack here ..... 
                    JsonNode columnarJson = JsonNode.Parse(body); 
                    JsonNode data = JsonColumnarCompression.DecompressColumnarToJson(columnarJson);

                    var transformedJson = JsonSerializer.Serialize(data);
                    var bytes = Encoding.UTF8.GetBytes(transformedJson);
                    context.Request.Body = new MemoryStream(bytes);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing request body: {ex.Message}");
                    // Optionally let the request pass through unmodified
                }
            }
            await _next(context);
        }
    }



    public class WrapResponseFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // You can modify incoming request here
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {

                JsonNode json = JsonNode.Parse(JsonSerializer.Serialize(objectResult.Value ,
                 new JsonSerializerOptions
                 {
                     PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                 }));

                var columnar = JsonColumnarCompression.CompressJsonToColumnar(json);
                // apply message pack here ..... 

                context.Result = new ObjectResult(columnar)
                {
                    StatusCode = objectResult.StatusCode
                };

            }
        }

        }
}


```