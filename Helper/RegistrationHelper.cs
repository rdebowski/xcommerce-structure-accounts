using System;
using System.Reflection;
using Sitecore.Commerce.EntityViews;
using Sitecore.Commerce.Plugin.Customers;

namespace XCommerce.StructuredAccounts.Helper
{
    public class RegistrationHelper : IRegistrationHelper
    {
        public ViewProperty SetCustomViewProperty(EntityView customerDetailsComponent, string propertyName, bool isReadOnly, Type type, bool IsRequired)
        {
            ViewProperty viewProperty = new ViewProperty();
            PropertyInfo property = typeof(Customer).GetProperty(propertyName);
            object propertyValue = customerDetailsComponent?.GetPropertyValue(propertyName);
            viewProperty.Name = propertyName;
            viewProperty.RawValue = propertyValue ?? string.Empty;
            viewProperty.IsReadOnly = isReadOnly;
            viewProperty.OriginalType = type.ToString();
            viewProperty.IsRequired = IsRequired;
            return viewProperty;
        }
    }
}
