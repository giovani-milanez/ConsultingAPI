using API.Data;
using System;
using System.Net;

namespace API.Exceptions
{
    public class APIException : SystemException
    {
        public APIException(string message)  : base(message)
        {
        }
    }
}
