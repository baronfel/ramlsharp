module Resources

open Parameters
open Common
open MarkdownString

type Schema = Schema of string

type TypedSchema<'a> = {
    mediaType : 'a
    value : string
}

type ExampleOfSchemaType<'a> = {
    mediaType : 'a
    value : string
}

type UserDocumentation = {
    title : string
    content : ItemOrInclude<MarkdownString.T> // can be an !incluuuude...
}

type ResourcePath = ResourcePath of string

type Body<'a> = {
    formParameters : Map<string, NamedParameter> option // only required if the mediaType is application/x-www-form-urlencoded or multipart/form-data
    schema : ItemOrInclude<TypedSchema<MediaType>> option
    example : ExampleOfSchemaType<MediaType>
}

type BodyMap = Map<MediaType, Body<MediaType>>
type HeaderName = HeaderName of string // there's some special {} or {?} wildcard magic that can happen here, so this is my reminder...

type Response = {
    description : string option
    body : BodyMap option
    headers : Map<HeaderName, NamedParameter> option
}

type Method = {
    description : MarkdownString.T option
    headers : Map<HeaderName, NamedParameter> option
    protocols : ProtocolType list option // overrides the protocols set at an API level, for this method only
    queryParameters : Map<string, NamedParameter> option
    body : BodyMap option // how can we make the value more....strong?
    responses : Map<HttpStatusCode, Response> option
}

type Resource = {
    displayName : string option // defaults to name if not present
    name : ResourcePath
    subResources : Resource list option
    uriParameters : Map<string, UriParameter> option // in the case of having a param in the uri, but no uri param, need to use the default defined above...
    baseUriParameters : Map<string,UriParameter> option // can be used to override the parent resources' params!
    methods : Map<HttpMethod, Method>  option
}
