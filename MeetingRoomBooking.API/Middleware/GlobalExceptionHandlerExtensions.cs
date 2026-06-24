using Microsoft.AspNetCore.Builder;

namespace MeetingRoomBooking.API.Middleware
{
    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    var loggerFactory = context.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                    var logger = loggerFactory?.CreateLogger("GlobalExceptionHandler");
                    logger?.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                    await GlobalExceptionHandler.HandleExceptionAsync(context, ex);
                }
            });
        }
    }
}
