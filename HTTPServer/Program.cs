using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static string fileName = "";
        static void Main(string[] args) // done
        {
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();

            //Start server
            // 1) Make server object on port 1000
            // 2) Start Server
            Server server = new Server(1000, "redirectionRules.txt"); 
            server.StartServer();
        }

        static void CreateRedirectionRulesFile() // done
        {
            // TODO: Create file named redirectionRules.txt
            fileName = "redirectionRules.txt";
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                sw.WriteLine("aboutus.html,aboutus2.html");
            }
            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
        }
         
    }
}
