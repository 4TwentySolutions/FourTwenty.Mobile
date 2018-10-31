namespace XamBasePacket.Bases.Validation
{
    public class ValidableClassAttribute : ValidationAttribute
    {
		public override string ErrorMessage { get; protected set; }
	    public override bool IsValid { get; protected set; } = true;


	    public override bool Validate(object value)
	    {
		    return true;
	    }
    }
}
