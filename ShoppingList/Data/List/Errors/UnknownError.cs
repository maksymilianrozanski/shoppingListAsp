using LaYumba.Functional;

namespace ShoppingList.Data.List.Errors
{
    public sealed class UnknownError : Error
    {
        public override string Message { get; } = "unknown error";
    }
}