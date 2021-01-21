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

        public static Option<Either<TL, TR2>> OptionEitherMap<TL, TR, TR2>(this Option<Either<TL, TR>> @this,
            Func<TR, TR2> f) =>
            @this.Map(i => i.Map(f));

        private static Try<Either<TL, TR>> TryEitherMap<TL, TR>(this
            Try<Either<TL, TR>> @this, Func<TR, Option<TR>> f, TL noneFallback)
            => @this.Map(j =>
                j.Bind(k => f(k)
                    .Map(m => (Either<TL, TR>) Right(m))
                    .GetOrElse(() => Left(noneFallback))));

        public static Option<Try<Either<TL, TR>>> OptionTryEitherMap<TL, TR>(this
            Option<Try<Either<TL, TR>>> @this, Func<TR, Option<TR>> f, TL noneFallback)
            => @this.Map(i => i.TryEitherMap(f, noneFallback));

        internal static Either<TL, TR> FlattenEither<TL, TR>(
            Either<TL, Either<TL, TR>> either) =>
            either.Bind(i => i);
    }
}