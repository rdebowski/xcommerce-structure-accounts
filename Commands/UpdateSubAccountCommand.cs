using System;
using System.Linq;
using System.Threading.Tasks;

using XCommerce.StructuredAccounts.Models;
using XCommerce.StructuredAccounts.Pipelines;

using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Customers;

namespace XCommerce.StructuredAccounts.Commands
{
    public class UpdateSubaccountCommand : CommerceCommand
    {
        private readonly IUpdateSubaccountPipeline _updateSubaccountPipeline;
        private readonly GetSubaccountsCommand _getSubaccountsCommand;

        public UpdateSubaccountCommand(IUpdateSubaccountPipeline updateSubaccountPipeline, IServiceProvider serviceProvider, GetSubaccountsCommand getSubaccountsCommand) : base(serviceProvider)
        {
            _updateSubaccountPipeline = updateSubaccountPipeline;
            _getSubaccountsCommand = getSubaccountsCommand;
        }

        public async Task<SubaccountAcctionsResult> Process(CommerceContext commerceContext, SubaccountModel subaccount, string mainaccountId)
        {
            Customer result;
            var subaccounts = await _getSubaccountsCommand.Process(commerceContext, mainaccountId);
            var validSubaccount = subaccounts.Subaccounts.FirstOrDefault(x => x.Id.Equals(subaccount.Id, StringComparison.OrdinalIgnoreCase));
            if (validSubaccount == null) { return new SubaccountAcctionsResult { Completed = false }; }
            using (CommandActivity.Start(commerceContext, (CommerceCommand)this))
            {
                result = await _updateSubaccountPipeline.Run(subaccount, commerceContext.GetPipelineContext());
                if (result == null)
                {
                    new SubaccountAcctionsResult { Completed = false };
                }
            }

            return new SubaccountAcctionsResult { Completed = true };
        }
    }
}
