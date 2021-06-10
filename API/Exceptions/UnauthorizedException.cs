namespace API.Exceptions
{
    public class UnauthorizedException : APIException
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
