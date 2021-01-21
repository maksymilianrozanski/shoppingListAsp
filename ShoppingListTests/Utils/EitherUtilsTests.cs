using LaYumba.Functional;
using NUnit.Framework;
using ShoppingList.Utils;

namespace ShoppingListTests.Utils
{
    public class EitherUtilsTests
    {
        [Test]
        public void ShouldReturnLeft()
        {
            Either<string, Either<string, int>> leftValue1 = "1234";
            Either<string, int> expected = "1234";
            var result = EitherUtils.FlattenEither(leftValue1);
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ShouldReturnRight()
        {
            Either<string, double> expected = 1234.5;
            Either<string, Either<string, double>> input = (Either<string, double>) 1234.5;
            var result = EitherUtils.FlattenEither(input);
            Assert.AreEqual(expected, result);
        }
    }
}