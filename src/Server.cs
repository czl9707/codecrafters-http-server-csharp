using System.Net;
using System.Net.Sockets;
using System.Text;
using MyHttpServer.Model.Request;
using MyHttpServer.Model.Response;

namespace MyHttpServer;

internal class Program
{
    internal static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, 4221);
        server.Start();

        while (true)
        {
            using var socket = server.AcceptSocket();
            using var stream = new NetworkStream(socket);
            using var writer = new StreamWriter(stream);
            using var reader = new StreamReader(stream);

            var requestString = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(requestString))
            {
                break;
            }

            var response = RequestHandler(
                HTTPRequest.FromString(requestString)
            );

            await writer.WriteAsync(response.AsString());
            await writer.FlushAsync();
        }
    }

    private static HTTPResponse RequestHandler(HTTPRequest request)
    {
        if (request.Path == "/")
        {
            return new OK();
        }
        else
        {
            return new NotFound();
        }
    }
}