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

type Raml = 
    | Version of float
    | Title of string
    | BaseUri of string // whole uri

type RamlDef = {
    version : float
    title : string
    baseUri : BaseUri
}