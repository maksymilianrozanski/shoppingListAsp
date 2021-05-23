namespace Authentication.Salt

open System
open System.Security.Cryptography
open System.Text

module SaltModule =

    type Salt = Salt of string
    type Password = Password of string
    type Hash = Hash of string

    let private joinSaltPassword salt password =
        match (salt, password) with
        | Salt s, Password p -> s + p

    let createSha (salt: Salt) (password: Password) : Hash =
        joinSaltPassword salt password
        |> Encoding.UTF8.GetBytes
        |> SHA256.Create().ComputeHash
        |> Convert.ToHexString
        |> Hash

    let verifySha (salt: Salt) shaCreator (savedSha: Hash) (password: Password) = savedSha = shaCreator salt password
