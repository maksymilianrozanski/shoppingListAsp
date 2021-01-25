using LaYumba.Functional;

namespace ShoppingList.Data.List.Errors
{
    public sealed class OtherError : Error
    {
        public override string Message { get; }

        public OtherError(string message)
        {
            Message = message;
        }
    }
}