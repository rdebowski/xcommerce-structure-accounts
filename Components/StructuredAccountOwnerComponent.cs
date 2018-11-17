using System.Collections.Generic;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Views;

namespace XCommerce.StructuredAccounts.Components
{
    public class StructuredAccountOwnerComponent : Component
    {
        public List<EntityReference> Subaccounts { get; set; } = new List<EntityReference>();
    }
}