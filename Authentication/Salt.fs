namespace Authentication.Salt

open System
open System.Security.Cryptography
open System.Text

module SaltModule =

    type Salt = Salt of string
    type Password = Password of string

    let private joinSaltPassword salt password =
        match (salt, password) with
        | Salt s, Password p -> s + p

    let createSha (salt: Salt) (password: Password) : string =
        joinSaltPassword salt password
        |> Encoding.UTF8.GetBytes
        |> SHA256.Create().ComputeHash
        |> Convert.ToHexString
