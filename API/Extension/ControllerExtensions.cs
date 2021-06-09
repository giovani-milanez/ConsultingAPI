using API.Data;
using API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace API.Extension
{
    public static class ControllerExtensions
    {
        public static IActionResult ApiResulFromException(this ControllerBase @this, Exception ex)
        {
            if (ex is NotFoundException)
            {
                @this.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return @this.NotFound(new ErrorResponse(ex.Message, @this.Response.StatusCode));
            }
            else if (ex is FieldValidationException fieldEx)
            {
                @this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return @this.BadRequest(new ErrorResponse(fieldEx.Message, @this.Response.StatusCode, fieldEx.Errors));                
            }
            else if (ex is APIException)
            {
                @this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return @this.BadRequest(new ErrorResponse(ex.Message, @this.Response.StatusCode));
            }
            else
            {
                @this.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return @this.StatusCode(@this.Response.StatusCode, new ErrorResponse(ex.Message, @this.Response.StatusCode));
            }
        }
        public static IActionResult ApiBadRequest(this ControllerBase @this, string message = "Error ao processar sua requisição")
        {
            @this.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return @this.BadRequest(new ErrorResponse(message, @this.Response.StatusCode));
        }

        public static IActionResult ApiNotFoundRequest(this ControllerBase @this, string message = "O recurso que você procura não foi encontrado")
        {
            @this.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return @this.NotFound(new ErrorResponse(message, @this.Response.StatusCode));
        }
    }
}
