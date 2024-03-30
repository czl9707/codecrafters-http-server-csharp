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
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);

            await RequestHandler(reader, writer);
        }
    }

    private static async Task RequestHandler(StreamReader reader, StreamWriter writer)
    {
        var buffer = new char[1024];
        int bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length);
        if (bytesRead == 0)
        {
            return;
        }

        var requestString = new StringBuilder().Append(buffer, 0, bytesRead).ToString();

        var request = HTTPRequest.FromString(requestString);
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
            request.Headers.TryGetValue("user-agent", out var content);
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

        // Console.WriteLine(response.AsString());
        await writer.WriteAsync(response.AsString());
        await writer.FlushAsync();
    }
}