using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using XCommerce.StructuredAccounts.Components;
using XCommerce.StructuredAccounts.Models;
using XCommerce.StructuredAccounts.Pipelines.Arguments;

using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Customers;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace XCommerce.StructuredAccounts.Pipelines.Blocks
{
    public class GetSubaccountsBlock : PipelineBlock<GetSubaccountsArgument, GetSubaccountsModel, CommercePipelineExecutionContext>
    {
        private readonly IGetCustomerPipeline _getCustomerPipeline;

        public GetSubaccountsBlock(IGetCustomerPipeline getCustomerPipeline)
        {
            _getCustomerPipeline = getCustomerPipeline;
        }

        public override async Task<GetSubaccountsModel> Run(GetSubaccountsArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg?.StructuredAccountOwnerEntityId).IsNotNullOrEmpty($"{this.Name}: The StructuredAccountOwnerEntityId can not be null");

            var structuredAccountOwner = await _getCustomerPipeline.Run(new GetCustomerArgument(arg.StructuredAccountOwnerEntityId, string.Empty), context);

            if (structuredAccountOwner?.HasComponent<StructuredAccountOwnerComponent>() != true
                || !structuredAccountOwner.GetComponent<StructuredAccountOwnerComponent>().Subaccounts.Any())
            {
                return new GetSubaccountsModel();
            }

            GetSubaccountsModel result = new GetSubaccountsModel();
            List<EntityReference> subaccountsEntityReference = structuredAccountOwner.GetComponent<StructuredAccountOwnerComponent>().Subaccounts;
            foreach (var subaccountEntityReference in subaccountsEntityReference)
            {
                Customer subaccount = await _getCustomerPipeline.Run(new GetCustomerArgument(subaccountEntityReference.EntityTarget, string.Empty), context);
                result.Subaccounts.Add(new SubaccountModel(subaccount));
            }

            return result;
        }
    }
}