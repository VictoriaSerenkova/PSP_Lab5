using System.Net.Security;
using System.Net.Sockets;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Text;

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.AspNetCore.Cors;
using System.Reflection.PortableExecutable;

namespace Lab5
{
    [EnableCors()]
    class Program
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static string pageData = "";


        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;
                

                // Print out some info about the request
                Console.WriteLine("Request");
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);

                if (req.HttpMethod == "GET")
                {
                    pageData =
            "    <form>" +
            "      <input id=\"num\">" +
            "    </form>";
                }
                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if (req.HttpMethod == "POST")
                {
                    Console.WriteLine(req.InputStream);
                    using(var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                    {
                        double reqBody = Convert.ToDouble(reader.ReadToEnd());
                        double answer = Teilor(reqBody);
                        Console.WriteLine(answer);
                        pageData = answer.ToString();
                    }
                }


                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, disableSubmit));
                resp.ContentType = "text/plain";
                //resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }

        public static double Teilor(double x)
        {
            double e = 0.001;
            if (x == 0.0)
            {
                return 0;
            }
            if (x < 0.0)
                x = -x;
            double t = x - 1;
            double u = t;
            double lnabsx = u;
            int n = 1;
            do
            {
                n++;
                u *= -((n - 1) * t) / n;
                lnabsx += u;
            } while (u > e || u < -e);
            return lnabsx;
        }
    }
}

//string responseStr = "<form method='POST'><span>Введите х</span><input></input><button type='submit'>Отправить</button></form>";