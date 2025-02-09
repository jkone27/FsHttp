﻿module FsHttp.Tests.``Alternative Syntaxes``

open FsUnit
open FsHttp
open FsHttp.DslCE
open FsHttp.Tests.Server

open NUnit.Framework

open Suave
open Suave.Cookie
open Suave.Operators
open Suave.Filters
open Suave.Successful


let [<TestCase>] ``Shortcut for GET works``() =
    use server = GET >=> request (fun r -> r.rawQuery |> OK) |> serve
    
    get (url @"?test=Hallo") { send }
    |> Response.toText
    |> should equal "test=Hallo"
