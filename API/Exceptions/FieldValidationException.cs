using API.Data;
using System.Collections.Generic;

namespace API.Exceptions
{
    public class FieldValidationException : APIException
    {
        public ICollection<FieldError> Errors { get; set; }
        public FieldValidationException(string message, FieldError errors) : base(message)
        {
            this.Errors = new List<FieldError>();
            this.Errors.Add(errors);
        }
    }
}
