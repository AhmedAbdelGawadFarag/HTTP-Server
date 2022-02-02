using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            //throw new NotImplementedException();
            // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
            headerLines = new List<string>();
            this.headerLines.Add($"content-type: {contentType}");
            this.headerLines.Add($"content-Length: {content.Length}");
            this.headerLines.Add($"Date: {DateTime.Now}");
            if (redirectoinPath != "")
            {
                headerLines.Add($"location: {redirectoinPath}");
            }

            // TODO: Create the request string
            string statusLine = GetStatusLine(code);
            string header = string.Empty;
            foreach (string line in headerLines)
            {
                header += line;
                header += "\r\n";
            }
            this.responseString = statusLine + "\r\n" + header +  "\r\n" + content;
            Console.WriteLine(statusLine);
        }

        private string GetStatusLine(StatusCode code)
        {
            // TODO: Create the response status line and return it
            string statusLine = string.Empty;
            string message = string.Empty;
            
            switch(code)
            {
                case StatusCode.OK: message = "200"; break;
                case StatusCode.NotFound: message = "404"; break ;
                case StatusCode.Redirect: message = "301"; break ;
                case StatusCode.BadRequest: message = "400"; break ;
                case StatusCode.InternalServerError: message = "500"; break ;
            }
             
            statusLine += Configuration.ServerHTTPVersion;
            statusLine += " " + message;
            statusLine += " " + code.ToString();
            return statusLine;
        }
    }
}
