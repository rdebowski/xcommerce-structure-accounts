using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using XCommerce.StructuredAccounts.Models;

using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.Plugin.Customers;

namespace XCommerce.StructuredAccounts.Commands
{
    public class DeleteSubaccountsCommand : CommerceCommand
    {
        private readonly IDeleteCustomerPipeline _delete;  
        private readonly GetSubaccountsCommand _getSubaccounts;
        public DeleteSubaccountsCommand(IServiceProvider serviceProvider, IDeleteCustomerPipeline delete, GetSubaccountsCommand getSubaccounts) : base(serviceProvider)
        {
            _delete = delete;
            _getSubaccounts = getSubaccounts;
        }

        public async Task<SubaccountAcctionsResult> Process(CommerceContext commerceContext, List<string> subaccountsId, string mainaccountId)
        {
            var subaccounts = await _getSubaccounts.Process(commerceContext, mainaccountId);          
            var validSubaccounts = subaccountsId.Where(x => subaccounts.Subaccounts.Any(y => y.Id == x ));

            if (!validSubaccounts.Any())
            {
                return new SubaccountAcctionsResult { Completed = false };
            }

            using (CommandActivity.Start(commerceContext, (CommerceCommand)this))
            {
                foreach (string id in validSubaccounts)
                {
                    await _delete.Run(new GetCustomerArgument(id, string.Empty), commerceContext.GetPipelineContext());
                }
            }

            return new SubaccountAcctionsResult{ Completed = true };
        }
    }
}
