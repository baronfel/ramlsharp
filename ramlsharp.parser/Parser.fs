﻿module parser

open FParsec
open AST

// PARSER HELPERS
type rParser<'a> = Parser<'a, unit>
let ws = spaces
let str s = pstring s
let getAllUntilNext s shouldSkip = charsTillStringCI s shouldSkip System.Int32.MaxValue
let advanceToEOL = restOfLine true

// PARAMETERS, ROUTE AND STATIC STRING
let routeParamParser : Parser<Parameter, unit> = between (str "{") (str "}")  (manySatisfy (fun c -> c <> '}')) |>> Parameter.Undetermined
let staticRouteParser : Parser<string, unit> = getAllUntilNext "/" true // either go until a slash or, if we're at the end of the line, grab the rest of the line
let lotsOfRouteParams : Parser<Parameter list, unit> = sepEndBy routeParamParser (str "/")

// PROTOCOLS
let httpParser =  stringReturn "http" Protocol.Http
let httpsParser  =  stringReturn "https" Protocol.Https
let protocolParser :rParser<Protocol> = httpsParser <|> httpParser

// BASEURI
let makeUri p d =
    {
       protocol = p
       domain = d
       staticRoutes = List.empty
       routeParams = List.empty
    }
let isSlashOrNewline c =
    c <> '/' && c<> '\n'

let domainParser : rParser<string> = between (str "://") (str "/") (manySatisfy (fun c ->  not (isSlashOrNewline c)))

let parseBaseUriString s =
    AST.defaultBaseUri


let baseUri = skipString "baseUri:" >>. ws >>. advanceToEOL |>> parseBaseUriString

// SIMPLE RAML PROPERTIES
let title = skipString "title:" >>. ws >>. advanceToEOL
let apiVersion = opt (skipString "version:" >>. ws >>. advanceToEOL) // api version isn't guaranteed, so....
let ramlVer = skipString "#%RAML" .>> ws >>. pfloat .>> advanceToEOL

let makeRamlDef ramlVersion title apiVersion uri = 
    {
        ramlVersion = ramlVersion
        title = title
        apiVersion = apiVersion
        baseUri = uri
    }

let raml : rParser<RamlDef> = pipe4 ramlVer title apiVersion baseUri makeRamlDef