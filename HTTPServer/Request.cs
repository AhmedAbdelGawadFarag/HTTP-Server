using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        public RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            //this.requestString = "POST /test HTTP/1.1\r\nHost: foo.exampleContent\r\nType: application /x-www-form-urlencodedContent\r\nLength: 27\r\n\r\nfield1 = value1 & field2 = value\r\n this is just a test";
            this.requestLines = requestString.Split(new string[] {Environment.NewLine }, StringSplitOptions.None );
            
            if (requestLines.Length < 3) return false;

            // Parse Request line
            string requestLine = requestLines[0];
            if (!ParseRequestLine(requestLine)) return false;
            
            if(this.method == RequestMethod.HEAD)
            {
                return true;
            }

            int headerSize = 1;
            List<string> headerLines_tmp = new List<string>();
            while(!ValidateBlankLine(requestLines[headerSize]))
            {
                headerLines_tmp.Add(requestLines[headerSize]);
                headerSize++;
            }
            
            // Validate blank line exists
            string blankLine = requestLines[headerSize];
            if (!ValidateBlankLine(blankLine) == true) return false;
            
            // Load header lines into HeaderLines dictionary
            headerLines = new Dictionary<string, string>();
            LoadHeaderLines(headerLines_tmp);


            // content of the request
            if (this.method == RequestMethod.POST)
            {
                headerSize++;
                int requestSize = this.requestLines.Length;
                contentLines = new string[requestSize - headerSize];
                int index = 0;
                for (int i = headerSize; i < requestSize; i++)
                {
                    this.contentLines[index] = requestLines[i];
                    index++;
                }
            }
            return true;
            
        }

        private bool ParseRequestLine(string requestLine)
        {
            string[] reqLine = requestLine.Split(' ');
            if(reqLine.Length != 3) return false;

            string METHOD = reqLine[0];
            string URI = reqLine[1];
            string http = reqLine[2];

            if (METHOD == "GET") this.method = RequestMethod.GET;
            else if (METHOD == "POST") this.method = RequestMethod.POST;
            else this.method = RequestMethod.HEAD;

            if (!ValidateIsURI(URI)) return false;
            this.relativeURI = URI;

            if (http == "HTTP/1.1") this.httpVersion = HTTPVersion.HTTP11;
            else if (http == "HTTP/1.0") this.httpVersion = HTTPVersion.HTTP10;
            else this.httpVersion = HTTPVersion.HTTP09;
            return true;
           // throw new NotImplementedException();
        }

        private bool ValidateIsURI(string uri) 
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines(List<string> headerLines_tmp)
        {
            foreach (string line in headerLines_tmp)
            {
                string[] splitString = line.Split(new string[] { ": " }, StringSplitOptions.RemoveEmptyEntries);
                headerLines.Add(splitString[0] , splitString[1]);
            }
            return true;
            //throw new NotImplementedException();
        }

        private bool ValidateBlankLine(string blankLine) 
        {
            if(String.IsNullOrEmpty(blankLine)) return true;
            return false;
            //throw new NotImplementedException();
        }

    }
}
