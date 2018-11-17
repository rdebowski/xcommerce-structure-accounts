using XCommerce.StructuredAccounts.Models;
using XCommerce.StructuredAccounts.Pipelines.Arguments;

using Microsoft.Extensions.Logging;

using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace XCommerce.StructuredAccounts.Pipelines
{
    public class GetSubaccountsPipeline : CommercePipeline<GetSubaccountsArgument, GetSubaccountsModel>, IGetSubaccountsPipeline
    {
        public GetSubaccountsPipeline(IPipelineConfiguration<IGetSubaccountsPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}