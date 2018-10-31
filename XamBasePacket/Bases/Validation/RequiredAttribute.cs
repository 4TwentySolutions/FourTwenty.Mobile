using System;
using System.Runtime.CompilerServices;
using XamBasePacket.Bases.Validation.Resources;

namespace XamBasePacket.Bases.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RequiredAttribute : ValidationAttribute
    {
        private readonly string _defaultErrorMessage = XamValidationResources.Required;


        public RequiredAttribute(string errorMessage = null, [CallerMemberName] string paramName = "")
        {
            ErrorMessage = errorMessage ?? string.Format(_defaultErrorMessage, paramName);
        }

        public sealed override string ErrorMessage { get; protected set; }

        public override bool IsValid { get; protected set; } = true;


        public override bool Validate(object value)
        {
            if (value is string s)
            {
                return IsValid = !string.IsNullOrWhiteSpace(s);
            }

            return IsValid = value != null;
        }
    }
}
