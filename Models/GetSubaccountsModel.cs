using System.Collections.Generic;

namespace XCommerce.StructuredAccounts.Models
{
    public class GetSubaccountsModel
    {
        public List<SubaccountModel> Subaccounts { get; set; } = new List<SubaccountModel>();
    }
}
