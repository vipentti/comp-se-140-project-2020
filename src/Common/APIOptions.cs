namespace APIGateway
{
    public record APIOptions
    {
        public string ApiGatewayUrl { get; init; } = "http://apigateway";
        public string HttpServerUrl { get; init; } = "http://httpserver";
    }
}
