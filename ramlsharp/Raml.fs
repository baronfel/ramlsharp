namespace ramlsharp.model

//TODO: resource types and traits are crazy, yo!
type RamlDefinition = {
    title : string
    version : string option // don't use if your API isn't versioned, or if the API definition itself doesn't change.
    baseUri : UriString // required, and would be nice to force FRC2396/RFC6570 compliance
    baseUriParameters : Map<string, UriParameter> option // param name to param details.
    protocols : ProtocolType list option // if not specified, infer from the baseUri
    mediaType : MediaType option
    schemas : ItemOrInclude<Map<string, Schema>> list option //schema name to schema definition. a list fo these because includes are a thing.
    documentation : UserDocumentation list option
    resources : Map<ResourcePath, Resource> list // resourcePath will want validation to ensure it starts with slash, so it's a separarate type
}