using Sitecore.Commerce.Core;

namespace XCommerce.StructuredAccounts.Pipelines.Arguments
{
    public class GetSubaccountsArgument : PipelineArgument
    {
        public GetSubaccountsArgument(string customerId)
        {
            StructuredAccountOwnerEntityId = customerId;
        }
        public string StructuredAccountOwnerEntityId { get; set; }
    }
}