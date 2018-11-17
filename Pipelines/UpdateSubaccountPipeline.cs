using XCommerce.StructuredAccounts.Models;

using Microsoft.Extensions.Logging;

using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Framework.Pipelines;

namespace XCommerce.StructuredAccounts.Pipelines
{
    public class UpdateSubaccountPipeline : CommercePipeline<SubaccountModel, Customer>, IUpdateSubaccountPipeline
    {
        public UpdateSubaccountPipeline(IPipelineConfiguration<IUpdateSubaccountPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
