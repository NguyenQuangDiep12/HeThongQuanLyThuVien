using HeThongQuanLyThuVien.Exceptions;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using System.Text.Json;

namespace HeThongQuanLyThuVien.Middlewares
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext _context, RequestDelegate _next)
        {
            try
            {
                await _next(_context);
            }catch(Exception ex)
            {
                _context.Response.ContentType = "application/json"; // gui ve content_type cua client ve dang "json"

                var statusCode = ex switch
                {
                    NotFoundException => (int)HttpStatusCode.NotFound, // 404 Reousrce not found
                    BadRequestException => (int)HttpStatusCode.BadRequest, // 400 Request not valid
                    ForbiddenException => (int)HttpStatusCode.Forbidden, // 403 resource forbid access
                    ConflictException => (int)HttpStatusCode.Conflict, // 409 Resource conflict
                    ValidationException => (int)HttpStatusCode.UnprocessableEntity, // 422 UnprocessableEntity
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized, // 401 Request not authorized
                    _ => (int)HttpStatusCode.InternalServerError, // 500 Error Server
                };

                _context.Response.StatusCode = statusCode;

                var response = new
                {
                    success = false,
                    statusCode,
                    error = ReasonPhrases.GetReasonPhrase(statusCode),
                    message = ex.Message,
                    path = _context.Request.Path,
                    timestamp = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(response);

                await _context.Response.WriteAsync(json);
            }
        }
    }
}
