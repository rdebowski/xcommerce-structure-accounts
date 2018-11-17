using XCommerce.StructuredAccounts.Models;

using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Framework.Pipelines;

namespace XCommerce.StructuredAccounts.Pipelines
{
    public interface IUpdateSubaccountPipeline : IPipeline<SubaccountModel, Customer, CommercePipelineExecutionContext>
    {
    }
}
