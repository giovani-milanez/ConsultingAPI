using System.Collections.Generic;

namespace API.Data
{    
    public class ErrorResponse
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public ICollection<FieldError> FieldErrors { get; set; }

        public ErrorResponse()
        {
            this.FieldErrors = new List<FieldError>();
        }

        public ErrorResponse(string message, int statusCode)
        {
            this.Message = message;
            this.StatusCode = statusCode;
            this.FieldErrors = new List<FieldError>();
        }

        public ErrorResponse(string message, int statusCode, ICollection<FieldError> fieldErrors) : this(message, statusCode)
        {
            FieldErrors = fieldErrors;
        }
    }
}
