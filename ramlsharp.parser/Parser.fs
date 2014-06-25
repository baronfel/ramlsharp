module parser

open FParsec
open AST

let ws = spaces
let str s = pstring s

let getProtocol s = 
    match s with
    | "http" -> Http
    | "https" -> Https
    | _ -> failwith "protocol not supported"

//let protocol : Parser<Protocol, unit> = str .<< (ws >>. str ":" .<< ws) |>> getProtocol
//
//let baseUri : Parser<BaseUri,unit> = skipString "baseUri: " >>. str .>> followedBy ":" 
let title : Parser<string,unit> = skipString "title: " >>. restOfLine true
let ramlVer : Parser<float,unit> = skipString "#%RAML " >>. pfloat .>> restOfLine true

let makeObj version title = // uri =
    printfn "making raml with version %f and title %s" version title
    {
        title = title
        version = version
        //baseUri = uri
        baseUri = defaultBaseUri
    }

let raml = pipe2 ramlVer title makeObj