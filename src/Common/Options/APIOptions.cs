namespace Common.Options
{
    public record APIOptions
    {
        public string ApiGatewayUrl { get; init; } = "http://apigateway";
        public string HttpServerUrl { get; init; } = "http://httpserver";
        public string OriginalServerUrl { get; init; } = "http://original";
        public string RabbitManagementUrl { get; init; } = "http://rabbitmq:15672";
    }
}
