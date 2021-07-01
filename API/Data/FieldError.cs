using System.Collections.Generic;

namespace API.Data
{
    public class FieldError
    {
        public string FieldName { get; set; }
        public ICollection<string> Errors { get; set; }

        public FieldError()
        {
            Errors = new List<string>();
        }

        public FieldError(string fieldName, string error) : this()
        {
            FieldName = fieldName;
            this.AddError(error);
        }

        public FieldError(string fieldName, List<string> errors)
        {
            FieldName = fieldName;
            Errors = errors;
        }

        public void AddError(string message)
        {
            this.Errors.Add(message);
        }
    }
}
