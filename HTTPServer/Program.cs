namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer server = new WebServer(5000,"web");
            server.Start();
            server.Listen();
        }
    }
}

