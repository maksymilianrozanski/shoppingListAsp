using LaYumba.Functional;
using Microsoft.FSharp.Core;
using static LaYumba.Functional.F;

namespace ShoppingList.Utils
{
    public static class EitherUtils
    {
        public static Either<T2, T1> FSharpChoiceToEither<T1, T2>(FSharpChoice<T1, T2> choice) =>
            choice switch
            {
                FSharpChoice<T1, T2>.Choice1Of2 right => Right(right.Item),
                FSharpChoice<T1, T2>.Choice2Of2 left => Left(left.Item)
            };
    }
}