module parser

open FParsec
open AST

// PARSER HELPERS
let ws = spaces
let str s = pstring s
let getAllUntil s shouldSkip = charsTillStringCI s shouldSkip System.Int32.MaxValue
let advanceToEOL = restOfLine true
let ThisOrEndOfLine parser = parser <|> advanceToEOL

// PARAMETERS, ROUTE AND QUERY STRING
let paramParser = str "{" >>. ThisOrEndOfLine (getAllUntil "}" true) |>> Parameter.Undetermined
let routeParamParser = paramParser
let staticRouteParser = ThisOrEndOfLine (getAllUntil "/" true) // either go until a slash or, if we're at the end of the line, grab the rest of the line

// PROTOCOLS
let httpParser : Parser<Protocol,unit> =  str "http" |>> fun _ -> Protocol.Http
let httpsParser =  str "https" |>> fun _ -> Protocol.Https
let protocolParser = httpParser <|> httpsParser

// BASEURI
let getValuesIfPresent l =
    match l with 
        | Some ls ->  List.choose id ls
        | None -> List.empty

let makeUri p d = // statics routeparameters = //TODO route params....
    {
       protocol = p
       domain = d
       staticRoutes = List.empty
       routeParams = List.empty
    }

let domainParser = str "://" >>. many1Satisfy (fun c -> (not (c = '/') && not (c = '\n')))
// TODO: need to account for static routes, like <domain>/api/{param}. we don't handle the /api yet...
let baseUri = skipString "baseUri: " >>. pipe2 protocolParser domainParser makeUri 

// SIMPLE RAML PROPERTIES
let title : Parser<string,unit> = skipString "title: " >>. advanceToEOL
let ramlVer : Parser<float,unit> = skipString "#%RAML " >>. pfloat .>> advanceToEOL

let makeObj version title uri = 
    printfn "making raml with version %f and title %s" version title
    {
        title = title
        version = version
        baseUri = uri
    }

let raml = pipe3 ramlVer title baseUri makeObj