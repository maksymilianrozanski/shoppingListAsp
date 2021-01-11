using System;
using LaYumba.Functional;
using Microsoft.FSharp.Core;
using SharedTypes.Entities;
using ShoppingData;
using static LaYumba.Functional.F;
using static ShoppingData.ShoppingListErrors;
using ShoppingListErrors = ShoppingData.ShoppingListErrors;

namespace ShoppingList.Utils
{
    public static class EitherUtils
    {
        public static Either<T2, T1> FSharpChoiceToEither<T1, T2>(FSharpChoice<T1, T2> choice) =>
            choice switch
            {
                FSharpChoice<T1, T2>.Choice1Of2 right => Right(right.Item),
                FSharpChoice<T1, T2>.Choice2Of2 left => Left(left.Item),
                _ => throw new MatchFailureException()
            };

        public static Option<Either<TL, TR2>> Map<TL, TR, TR2>(this Option<Either<TL, TR>> @this,
            Func<TR, TR2> f) =>
            @this.Map(i => i.Map(f));
    }
}