using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using DSS.Models.DTOs;

namespace DSS.Misc
{
    public class CustomExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            int statusCode = 500;
            string message = context.Exception.Message;

            switch (context.Exception)
            {
                case KeyNotFoundException:
                    statusCode = 404;
                    break;
                case ArgumentException:
                    statusCode = 400;
                    break;
                case UnauthorizedAccessException:
                    statusCode = 401;
                    break;
                case InvalidOperationException:
                    statusCode = 409;
                    break;
            }
            context.Result = new ObjectResult(new ErrorObjectDto
            {
                ErrorNumber = statusCode,
                ErrorMessage = context.Exception.Message
            })
            {
                StatusCode = statusCode
            };
        }
    }
}