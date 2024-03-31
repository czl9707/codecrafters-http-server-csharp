using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.CommandLine;
using MyHttpServer.Model.Request;
using MyHttpServer.Model.Response;
using System.ComponentModel;

namespace MyHttpServer;

internal class Program
{
    internal static string? Directory { get; set; }
    internal static async Task Main(string[] args)
    {
        var rootCommand = new RootCommand("MyHttpServer");
        var directoryOption = new Option<string?>("--directory",
            description: "An option declare directory the server hosted in.",
            getDefaultValue: () => null);
        rootCommand.AddOption(directoryOption);

        rootCommand.SetHandler((context) =>
        {
            var directory = context.ParseResult.GetValueForOption(directoryOption);
            Program.Directory = directory;
        });
        rootCommand.Invoke(args);

        TcpListener server = new TcpListener(IPAddress.Any, 4221);
        server.Start();

        while (true)
        {
            var socket = server.AcceptSocket();
            var _ = Task.Run(() => RequestHandler(socket));
        }
    }

    private static async Task<string> ReadStringAsync(StreamReader reader)
    {
        try
        {
            var buffer = new char[reader.BaseStream.CanSeek ? reader.BaseStream.Length : 1024];
            int bytesRead = await reader.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead == 0)
            {
                return String.Empty;
            }

            return new StringBuilder().Append(buffer).ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    private static async Task RequestHandler(Socket socket)
    {
        using (socket)
        {
            using var stream = new NetworkStream(socket);
            using var reader = new StreamReader(stream);
            using var writer = new StreamWriter(stream);

            var requestString = await ReadStringAsync(reader);
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
            else if (request.Path.StartsWith("/files"))
            {
                var filename = request.Path.Substring("/files/".Length);
                if (Program.Directory is null)
                {
                    response = new NotFound();
                }
                else if (!File.Exists(Path.Combine(Program.Directory, filename)))
                {
                    response = new NotFound();
                }
                else
                {
                    var fullPath = Path.Combine(Program.Directory, filename);
                    var fileContent = await ReadStringAsync(
                        new StreamReader(File.Open(fullPath, FileMode.Open))
                    );

                    response = new OK();
                    response.Content = fileContent;
                    response.Headers.Add("Content-Length", fileContent.Length.ToString());
                    response.Headers.Add("Content-Type", "application/octet-stream");
                }
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
}