using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;
       
        public Server(int portNumber, string redirectionMatrixPath) // done
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
             this.LoadRedirectionRules(redirectionMatrixPath);

            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.serverSocket.Bind(new IPEndPoint(IPAddress.Any, portNumber));
        }

        public void StartServer() // done
        {
            // TODO: Listen to connections, with large backlog.
            int backlog = 100;
            this.serverSocket.Listen(backlog);

            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = serverSocket.Accept();
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                
                newThread.Start(clientSocket);
            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket ClientSocket = (Socket)obj;

            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            ClientSocket.ReceiveTimeout = 0;

            // TODO: receive requests in while true until remote client closes the socket.
            int recivedLength;
            byte[] data = new byte[1024];

            while (true)
            {
                try
                {
                    // TODO: Receive request
                    recivedLength = ClientSocket.Receive(data);
                    
                    // TODO: break the while loop if receivedLen==0
                    if (recivedLength == 0) break;
                    
                    // TODO: Create a Request object using received request string
                    Request request = new Request(Encoding.ASCII.GetString(data));

                    // TODO: Call HandleRequest Method that returns the response
                    Response response =  HandleRequest(request);
               
                    // TODO: Send Response back to client   
                    ClientSocket.Send(Encoding.ASCII.GetBytes(response.ResponseString));
   
                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
           
            }

            // TODO: close client socket
            ClientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            Response response = null;
            string content;
            try
            {
                //TODO: check for bad request 
                if (!request.ParseRequest())
                {
                    content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.BadRequestDefaultPageName));
                    return response = new Response(StatusCode.BadRequest, "text/html", content, "");
                }
                if(request.method == RequestMethod.HEAD)
                {
                    content = "";
                    return response = new Response(StatusCode.OK, "headers", content, "");
                }
                string pagepath = request.relativeURI;
                content = LoadDefaultPage(pagepath);
                if(content == "")
                {
                    content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.NotFoundDefaultPageName));
                    return response = new Response(StatusCode.NotFound, "text/html", content, "");
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.

                //TODO: check for redirect
                string redirectpagepath = GetRedirectionPagePathIFExist(pagepath);

                if(redirectpagepath == "")
                {
                    content = File.ReadAllText(Path.Combine(Configuration.RootPath, pagepath.Remove(0,1)));
                    return response = new Response(StatusCode.OK, "text/html", content, "");
                }
                else
                {
        
                    content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.RedirectionDefaultPageName));
                    return response = new Response(StatusCode.Redirect, "text/html", content, redirectpagepath);
                }
              
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                content = content = File.ReadAllText(Path.Combine(Configuration.RootPath, Configuration.InternalErrorDefaultPageName));
                return response = new Response(StatusCode.InternalServerError, "text/html", content, "");
                // TODO: in case of exception, return Internal Server Error. 

            }
    
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            relativePath = relativePath.Remove(0, 1);
            foreach (KeyValuePair<string,string> rule in Configuration.RedirectionRules)
            {
                if(rule.Key == relativePath) return rule.Value;
            }
            return String.Empty;
           
        }

        private string LoadDefaultPage(string defaultPageName) // done
        {
            string filepath = Path.Combine(Configuration.RootPath,defaultPageName.Remove(0,1));
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if(File.Exists(filepath))
            {
               string content = File.ReadAllText(filepath);
               return content;
            }
            
            return string.Empty;
            // else read file and return its content
           
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                // then fill Configuration.RedirectionRules dictionary 
                Configuration.RedirectionRules = new Dictionary<string, string>();
                foreach(string line in File.ReadLines(filePath))
                {
                    string[] tmpLine = line.Split(new char[] { ',' });
                    Configuration.RedirectionRules.Add(tmpLine[0] , tmpLine[1]);
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
