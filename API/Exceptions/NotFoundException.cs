namespace API.Exceptions
{
    public class NotFoundException : APIException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
