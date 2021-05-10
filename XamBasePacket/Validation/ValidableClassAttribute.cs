using System.ComponentModel.DataAnnotations;

namespace XamBasePacket.Validation
{

    public class ValidableClassAttribute
        : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return true;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return ValidationResult.Success;
        }
    }

}
