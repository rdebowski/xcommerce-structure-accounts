using Sitecore.Commerce.EntityViews;
using System;

namespace XCommerce.StructuredAccounts.Helper
{
    public interface IRegistrationHelper
    {
        ViewProperty SetCustomViewProperty(EntityView customerDetailsComponent, string propertyName, bool isReadOnly, Type type, bool IsRequired);
    }
}
