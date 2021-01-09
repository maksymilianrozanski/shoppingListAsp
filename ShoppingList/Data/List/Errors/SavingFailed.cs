using LaYumba.Functional;

namespace ShoppingList.Data.List.Errors
{
    public sealed class SavingFailed : Error
    {
        public override string Message { get; } = "saving failed";
    }
}