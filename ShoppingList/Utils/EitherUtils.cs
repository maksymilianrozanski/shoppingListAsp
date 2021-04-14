using System;
using System.Runtime.InteropServices.ComTypes;
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

        public static Option<Either<TL, TR2>> OptionEitherMapBind<TL, TR, TR2>(this Option<Either<TL, TR>> @this,
            Func<TR, Either<TL, TR2>> f) =>
            @this.Map(i => i.Bind(f));

        public static Try<Option<Either<TL, TR2>>> TryOptionEitherMap<TL, TR, TR2>(
            this Try<Option<Either<TL, TR>>> @this, Func<TR, TR2> f) =>
            @this.TryOptionMap(j => j.Map(f));

        public static Try<Option<TR>> TryOptionMap<T, TR>(this Try<Option<T>> @this, Func<T, TR> f) =>
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

        public static Either<TL, Option<T2>> EitherOptionMap<TL, T1, T2>(this Either<TL, Option<T1>> @this,
            Func<T1, T2> f) =>
            @this.Map(i => i.Map(f));

        public static Either<TL, Option<T2>> EitherOptionBind<TL, T1, T2>(this Either<TL, Option<T1>> @this,
            Func<T1, Option<T2>> f) =>
            @this.Map(i => i.Bind(f));

        public static Either<TL2, TR2> Bimap<TL1, TL2, TR1, TR2>(this Either<TL1, TR1> @this, Func<TL1, TL2> leftFunc,
            Func<TR1, TR2> rightFunc) =>
            @this.Match(l => (Either<TL2, TR2>) leftFunc(l), r => rightFunc(r));

        public static Either<TL, T> NoneToEitherLeft<TL, T>(this
            Either<TL, Option<T>> option, TL errorValue) =>
            option.Bind(i =>
                i.Match<Either<TL, T>>(
                    () => Left(errorValue),
                    some => Right(some)));

        internal static Either<TL, TR> FlattenEither<TL, TR>(
            Either<TL, Either<TL, TR>> either) =>
            either.Bind(i => i);
    }
}