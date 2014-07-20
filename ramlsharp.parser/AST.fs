module AST

type Protocol = Http|Https

type Parameter = 
    | Undetermined of string 
    | String of string
    | Number of string 

type BaseUri = {
    protocol : Protocol  // http/https
    domain : string // example.what.com
    staticRoutes : string list  // /api/test/etc Assumption: that these static parts will ALWAYS be before any parameters
    routeParams : Parameter list // /{version}/{resource}
}
let defaultBaseUri = {
    protocol = Http
    domain  = ""
    staticRoutes = List.empty
    routeParams = List.empty
}

type RamlDef = {
    ramlVersion : float
    apiVersion : string option
    title : string
    baseUri : BaseUri
}
