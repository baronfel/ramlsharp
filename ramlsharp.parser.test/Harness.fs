namespace ramlsharp.parser.test

open NUnit.Framework
open FsUnit
open parser
open FParsec
open AST

module TestHarness =
    let runWithCont parser input successCont failureFormat =
        match run parser input with
        | Success (result, userstate, position) -> successCont result
        | Failure (errStr, err, userstate) -> Assert.Fail((failureFormat errStr err userstate))


    let generalErrString es _ _ =
        sprintf "%s" es

    let generalSuccess expected result =
        should equal expected result

    [<TestFixture>]
    type ``given a text raml spec`` ()=
        let text = System.IO.File.ReadAllText(@"..\..\partial.raml")
    
        let verifyBaseUri uri =
            uri.protocol |> should equal Protocol.Http
            uri.domain.Split '.' |> Array.length |> should equal 3 // 3 parts to that domain
            uri.routeParams |> List.length |> should be (greaterThan 0)

        let verifyDef (r:RamlDef) =
            r.version |> should equal 0.8
            r.title |> should equal "World Music API"
            r.baseUri |> verifyBaseUri
            

        [<Test>] 
        member x.``can read that spec`` ()=
            let success a =
                verifyDef a
                printfn "%A" a 
                
            runWithCont parser.raml text success generalErrString


    [<TestFixture>]
    type ``given a version number``()=
        [<Test>]
        member x.``can parse version number``()=
            let v = 0.8m
            let versionStr = "#%RAML " + v.ToString("0.0")

            let success a =
                should (equalWithin 0.1) v a

            runWithCont parser.ramlVer versionStr success generalErrString
            

    [<TestFixture>]
    type ``given an api title`` ()=
        [<Test>]
        member x.``can parse that title`` ()=
            let t = "World Music API"
            let text = sprintf "title: %s" t   

            runWithCont parser.title text (generalSuccess t) generalErrString

    [<TestFixture>]
    type ``given a route param``()=
        [<Test>]
        member x.``can parse that route param`` ()=
            let text = "{version}" // this is what's left from a baseuri after we get the protocol and domain
            let expected = Parameter.Undetermined "version"
            runWithCont parser.routeParamParser text (generalSuccess expected) generalErrString

        [<Test>]
        member x.``can parse several route parameters in a row``()=
            let text = "/{version}/{other}/"
            let success pars =
                pars |> List.length |> should equal 2
                pars |> should equal [Parameter.Undetermined "version"; Parameter.Undetermined "other"]

            runWithCont (many  parser.routeParamParser) text success generalErrString

    [<TestFixture>]
    type ``given a base uri string`` ()=
        [<Test>]
        member x.``can parse simple uri``()=
            let uri = "baseUri: http://example.api.com"
            let expected = {
                protocol = Protocol.Http
                domain = "example.api.com"
                staticRoutes = List.empty
                routeParams = List.empty
            }
            runWithCont parser.baseUri uri (generalSuccess expected) generalErrString

        [<Test>]
        member x.``can parse a standard uri string with slash``()=
            let uri = "baseUri: http://example.api.com/"
            let expected = {
                protocol = Protocol.Http
                domain = "example.api.com"
                staticRoutes = List.empty
                routeParams = List.empty
            }

            runWithCont parser.baseUri uri (generalSuccess expected) generalErrString
        
        [<Test>]
        member x.``can parse a uri with a route param``()=
            let uri = "baseUri: http://example.api.com/{version}/"
            let expected = {
                protocol = Protocol.Http
                domain = "example.api.com"
                staticRoutes = List.empty
                routeParams = [Parameter.Undetermined "version"]
            }

            runWithCont parser.baseUri uri (generalSuccess expected) generalErrString

        [<Test>]
        member x.``can parse a baseuri with a static route before params``()=
            let uri = "baseUri: http://example.api.com/api/{version}/"
            let expected = {
                protocol = Protocol.Http
                domain = "example.api.com"
                staticRoutes = ["api"]
                routeParams = [Parameter.Undetermined "version"]
            }

            runWithCont parser.baseUri uri (generalSuccess expected) generalErrString