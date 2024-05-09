using System;

namespace bloggit.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException() : base() { }

        public DomainException(string message) : base(message) { }

        public DomainException(string message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public int ErrorCode { get; set; }

    }
}
