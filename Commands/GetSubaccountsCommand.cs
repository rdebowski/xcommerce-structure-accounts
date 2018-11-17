using System;
using System.Threading.Tasks;

using XCommerce.StructuredAccounts.Models;
using XCommerce.StructuredAccounts.Pipelines;
using XCommerce.StructuredAccounts.Pipelines.Arguments;

using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;

namespace XCommerce.StructuredAccounts.Commands
{
    public class GetSubaccountsCommand : CommerceCommand
    {
        private readonly IGetSubaccountsPipeline _getSubaccountsPipeline;

        public GetSubaccountsCommand(IGetSubaccountsPipeline getSubaccountsPipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _getSubaccountsPipeline = getSubaccountsPipeline;
        }

        public async Task<GetSubaccountsModel> Process(CommerceContext commerceContext, string customerId)
        {
            GetSubaccountsModel subaccounts;
            
            using (CommandActivity.Start(commerceContext, (CommerceCommand)this))
            {
                subaccounts = await _getSubaccountsPipeline.Run(new GetSubaccountsArgument(customerId), new CommercePipelineExecutionContextOptions(commerceContext));
            }

            return subaccounts;
        }
    }
}