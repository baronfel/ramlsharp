module AST

type ParameterType = 
    | String
    | Number

type Protocol = Http|Https

type Parameter = 
    |Name of string
    |Type of ParameterType

type BaseUri = {
    protocol : Protocol 
    domain : string
    routeParams : Parameter list
}
let defaultBaseUri = {
    protocol = Http
    domain  = ""
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