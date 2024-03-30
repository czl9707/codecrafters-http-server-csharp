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

            // Console.WriteLine(response.AsString());
            await writer.WriteAsync(response.AsString());
            await writer.FlushAsync();
        }
    }

    private static HTTPResponse RequestHandler(HTTPRequest request)
    {
        HTTPResponse response;
        if (request.Path == "/")
        {
            response = new OK();
        }
        else if (request.Path.StartsWith("/echo"))
        {
            var content = request.Path.Substring("/echo/".Length);
            response = new OK();
            response.Content = content;
            response.Headers.Add("Content-Length", content.Length.ToString());
            response.Headers.Add("Content-Type", "text/plain");
        }
        else if (request.Path.StartsWith("/user-agent"))
        {
            request.Headers.TryGetValue("User-Agent", out var content);
            content = content ?? string.Empty;

            response = new OK();
            response.Content = content;
            response.Headers.Add("Content-Length", content.Length.ToString());
            response.Headers.Add("Content-Type", "text/plain");
        }
        else
        {
            response = new NotFound();
        }

        return response;
    }
}