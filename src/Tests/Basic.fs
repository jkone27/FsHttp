﻿module FsHttp.Tests.Basic

open System
open System.Threading

open FsUnit
open FsHttp
open FsHttp.DslCE
open FsHttp.Tests.TestHelper
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Operators
open Suave.Filters
open Suave.Successful


let [<TestCase>] ``Synchronous calls are invoked immediately``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    get (url @"?test=Hallo")
    |> Request.send
    |> Response.toText
    |> should equal "test=Hallo"


let [<TestCase>] ``Asynchronous calls are sent immediately``() =

    let mutable time = DateTime.MaxValue

    use server =
        GET
        >=> request (fun r ->
            time <- DateTime.Now
            r.rawQuery |> OK)
        |> serve

    let req =
        get (url "?test=Hallo")
        |> Request.sendAsync
    
    Thread.Sleep 3000

    req
    |> Async.RunSynchronously
    |> Response.toText
    |> should equal "test=Hallo"

    (DateTime.Now - time > TimeSpan.FromSeconds 2.0) |> should equal true


let [<TestCase>] ``Split URL are interpreted correctly``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve

    http { GET (url @"
                    ?test=Hallo
                    &test2=Welt")
    }
    |> Response.toText
    |> should equal "test=Hallo&test2=Welt"


let [<TestCase>] ``Smoke test for a header``() =
    use server = GET >=> request (header "accept-language" >> OK) |> serve

    let lang = "zh-Hans"
    
    http {
        GET (url @"")
        AcceptLanguage lang
    }
    |> Response.toText
    |> should equal lang


let [<TestCase>] ``ContentType override``() =
    use server = POST >=> request (header "content-type" >> OK) |> serve

    let contentType = "text/xxx"

    http {
        POST (url @"")
        body
        ContentType contentType
        text "hello world"
    }
    |> Response.toText
    |> should contain contentType

