using System.Net;

namespace WebApplication1.Common
{
    public class BusinessException : Exception
    {
        public int LogNo { get; set; }

        public BusinessException() : base()
        {

        }
        public BusinessException(string message) : base(message)
        {

        }
        public BusinessException(int logno, string message, Exception ex) : base(message, ex)
        {
            this.LogNo = logno;
        }
    }


    public class DALException : Exception
    {
        public int LogNo { get; set; }

        public DALException() : base()
        {

        }
        public DALException(string message) : base(message)
        {

        }
        public DALException(int logno, string message, Exception ex) : base(message, ex)
        {
            this.LogNo = logno;
        }
    }


    public class HttpException : Exception
    {
        private readonly int httpStatusCode;

        public HttpException() : base()
        {

        }

        public HttpException(int httpStatusCode)
        {
            this.httpStatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode)
        {
            this.httpStatusCode = (int)httpStatusCode;
        }

        public HttpException(int httpStatusCode, string message) : base(message)
        {
            this.httpStatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            this.httpStatusCode = (int)httpStatusCode;
        }

        public HttpException(int httpStatusCode, string message, Exception inner) : base(message, inner)
        {
            this.httpStatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode, string message, Exception inner) : base(message, inner)
        {
            this.httpStatusCode = (int)httpStatusCode;
        }

        public int StatusCode { get { return this.httpStatusCode; } }
    }
}
