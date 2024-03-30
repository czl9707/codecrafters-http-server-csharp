using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;


internal class Program
{
    public static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 4221);
        server.Start();

        Console.WriteLine("server start");

        while (true)
        {
            using var socket = server.AcceptSocket();
            // using var stream = new NetworkStream(socket);
            // using var writer = new StreamWriter(stream);
            // using var reader = new StreamReader(stream);
            Console.WriteLine("socket");

            var OKResponse = "HTTP/1.1 200 OK\r\n\r\n";
            var OKResponseBytes = Encoding.UTF8.GetBytes(OKResponse);
            // await writer.WriteAsync(OKResponse);
            // await writer.FlushAsync();
            await socket.SendAsync(OKResponseBytes, SocketFlags.None);

            Console.WriteLine("socket close");
        }
    }
}