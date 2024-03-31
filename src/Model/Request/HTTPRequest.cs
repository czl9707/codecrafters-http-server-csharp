namespace MyHttpServer.Model.Request;

internal class HTTPRequest
{
    internal string Method { get; init; }
    internal string Path { get; init; }
    internal string HttpVersion { get; init; }
    internal string Body { get; init; }
    internal Dictionary<string, string> Headers { get; init; }

    protected HTTPRequest(string method, string path, string httpVersion = "1.1", Dictionary<string, string>? headers = null, string? body = null)
    {
        this.Method = method;
        this.Path = path;
        this.HttpVersion = httpVersion;
        this.Headers = headers ?? new Dictionary<string, string>();
        this.Body = body ?? String.Empty;
    }

    internal static HTTPRequest FromString(string content)
    {
        var chunks = content.Split("\r\n\r\n", 2);
        var headersChunk = chunks.ElementAt(0);
        var bodyChunk = chunks.ElementAt(1);

        var linesEnumerator = headersChunk.Split("\r\n").AsEnumerable().GetEnumerator();
        string line;
        linesEnumerator.MoveNext();
        line = linesEnumerator.Current;

        var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var method = parts.ElementAt(0);
        var path = parts.ElementAt(1);
        var version = parts.ElementAt(2);

        var headers = new Dictionary<string, string>();
        while (linesEnumerator.MoveNext())
        {
            line = linesEnumerator.Current;
            if (string.IsNullOrEmpty(line))
            {
                break;
            }
            var kvpStrings = line.Split(":", 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            headers.Add(
                kvpStrings.ElementAt(0).ToLowerInvariant(),
                kvpStrings.ElementAt(1).ToLowerInvariant());
        }

        return new HTTPRequest(method, path, version, headers, bodyChunk);
    }
}