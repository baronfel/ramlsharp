module parser

open FParsec
open AST

// PARSER HELPERS
type WhitespaceContext = {
    spaces: int
} with static member Default = {spaces = 0}

let pushIndent c = { c with spaces = c.spaces + 2}
let popIndent c = {c with spaces = c.spaces - 2}

type rParser<'a> = Parser<'a, WhitespaceContext>

let ws = spaces
let indent = 
    parse {
        let! state = getUserState
        do! manyMinMaxSatisfy state.spaces state.spaces (isAnyOf " ") |>> ignore
    }

let str s = pstring s
let getAllUntilNext s shouldSkip = charsTillStringCI s shouldSkip System.Int32.MaxValue
let advanceToEOL = restOfLine true
let listMarker = str "- " |>> ignore

// parses things of the form NAME: to return NAME
let keyName = manyCharsTill anyChar (anyOf ":") .>> advanceToEOL
// TRAITS - to parse traits, we'll need to push in an indent level, and then parse a list of things....
let makeTrait n = 
    {
        name = n
    }

let traitParser = indent >>. listMarker >>. keyName |>> makeTrait
let traitsList = skipString "traits:" >>. ws >>. many traitParser

// PARAMETERS, ROUTE AND STATIC STRING
let routeParamParser : rParser<Parameter> = between (str "{") (str "}")  (manySatisfy (fun c -> c <> '}')) |>> Parameter.Undetermined
let staticRouteParser : rParser<string> = getAllUntilNext "/" true // either go until a slash or, if we're at the end of the line, grab the rest of the line
let lotsOfRouteParams : rParser<Parameter list> = sepEndBy routeParamParser (str "/")

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
        traits = List.empty
    }

let raml : rParser<RamlDef> = pipe4 ramlVer title apiVersion baseUri makeRamlDef