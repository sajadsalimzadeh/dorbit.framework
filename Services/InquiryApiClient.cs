using System;
using Dorbit.Framework.Configs;
using Dorbit.Framework.Utils.Http;

namespace Dorbit.Framework.Services;

public class InquiryApiClient(IServiceProvider serviceProvider) : HttpClientApi<ConfigInquiryApi>(serviceProvider)
{
}