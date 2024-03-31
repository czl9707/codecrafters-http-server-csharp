using System.Text;

namespace MyHttpServer.Model.Response;

internal abstract class HTTPResponse
{
    private const string NEWLINE = "\r\n";
    internal string Content { get; set; } = string.Empty;
    internal Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    internal string HttpVersion { get; private set; }
    internal abstract int Code { get; }
    internal abstract string Name { get; }
    protected HTTPResponse(string httpVersion = "1.1")
    {
        this.HttpVersion = httpVersion;
    }

    internal string AsString()
    {
        var sb = new StringBuilder();
        sb.Append($"HTTP/{this.HttpVersion} {this.Code} {this.Name}{NEWLINE}");

        foreach (var (key, value) in this.Headers)
        {
            sb.Append($"{key}: {value}{NEWLINE}");
        }
        sb.Append(NEWLINE);
        sb.Append(this.Content);

        return sb.ToString();
    }
}

internal class OK : HTTPResponse
{
    internal override int Code => 200;
    internal override string Name => "OK";
    public OK(string httpVersion = "1.1") : base(httpVersion) { }
}

internal class NotFound : HTTPResponse
{
    internal override int Code => 404;
    internal override string Name => "Not Found";
    public NotFound(string httpVersion = "1.1") : base(httpVersion) { }
}

internal class Created : HTTPResponse
{
    internal override int Code => 201;
    internal override string Name => "Created";
    public Created(string httpVersion = "1.1") : base(httpVersion) { }
}

