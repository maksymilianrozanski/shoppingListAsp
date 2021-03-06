module Authentication.Tests

open Authentication.Salt
open NUnit.Framework
open SaltModule

[<SetUp>]
let Setup () = ()

let salt = Salt "PasswordSalt"

let password = Password "password"

[<Test>]
let shouldGenerateSha () =
    let expected =
        Hash "296BA429BD9F8B773D986C66C1D4AEEB659B442784D8F4E8752A3B6286997AAC"

    let result = createSha salt password
    Assert.AreEqual(expected, result, "should create SHA with expected value")

[<Test>]
let ``should return true for equal saved and generated SHA`` () =
    let creator _ _ = Hash "A111"
    let savedSha = Hash "A111"
    let result = verifySha salt creator savedSha password
    Assert.True(result)

[<Test>]
let ``should return false for different saved and generated SHA`` () =
    let creator _ _ = Hash "A111"
    let savedSha = Hash "B111"
    let result = verifySha salt creator savedSha password
    Assert.False(result)
