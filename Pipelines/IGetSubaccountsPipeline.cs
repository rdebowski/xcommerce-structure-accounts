using XCommerce.StructuredAccounts.Models;
using XCommerce.StructuredAccounts.Pipelines.Arguments;

using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace XCommerce.StructuredAccounts.Pipelines
{
    public interface IGetSubaccountsPipeline : IPipeline<GetSubaccountsArgument, GetSubaccountsModel, CommercePipelineExecutionContext>
    {
    }
}