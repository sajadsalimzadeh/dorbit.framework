using System.Net;
using Dorbit.Framework.Configs;
using Dorbit.Framework.Utils.Http;
using Serilog;

namespace Dorbit.Framework.Extensions;

public static class ConfigClientExtensions
{
    public static HttpHelper GetHttpHelper(this ConfigClientApi config, string apiKeyHeader = null)
    {
        return config.GetHttpHelper(null, apiKeyHeader);
    }

    public static HttpHelper GetHttpHelper(this ConfigClientApi config, ILogger logger, string apiKeyHeader = null)
    {
        var http = new HttpHelper(config.ApiUrl);
        if (config.ApiKey is not null)
        {
            http.AuthorizationToken = config.ApiKey.GetDecryptedValue();
        }

        if (config.Proxy.IsNotNullOrEmpty())
        {
            http.HttpClientHandler.Proxy = new WebProxy(config.Proxy);
            http.HttpClientHandler.UseProxy = true;
        }

        if (logger is not null)
        {
            http.OnException += (ex, args) => { logger.Error("Http Client {@Exception} {@Request} {@Response} {@Content}", ex, args.Request, args.Response, args.Content); };
        }

        return http;
    }
}