using System;

namespace XamBasePacket.Bases.Validation
{
    public abstract class ValidationAttribute : Attribute
    {
        public abstract string ErrorMessage { get; protected set; }
        public abstract bool IsValid { get; protected set; }

        public string ErrorResourceName { get; set; }
        public Type ErrorResourceType { get; set; }
        public abstract bool Validate(object value);
    }
}
