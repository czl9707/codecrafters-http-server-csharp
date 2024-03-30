namespace MyHttpServer.Model.Response;

internal abstract class HTTPResponse
{
    internal string HttpVersion { get; private set; }
    internal abstract int Code { get; }
    internal abstract string Name { get; }
    protected HTTPResponse(string httpVersion = "1.1")
    {
        this.HttpVersion = httpVersion;
    }

    internal string AsString()
    {
        return $"HTTP/{this.HttpVersion} {this.Code} {this.Name}\r\n\r\n";
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