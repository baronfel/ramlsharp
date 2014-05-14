namespace ramlsharp.parser

open ramlsharp.model

module Parser=
    let Load text =
        {
            baseUri = "test.com"
            title = "test"
            version = Some "1.2"
            baseUriParameters = None
            protocols = None
            mediaType = None
            schemas = None
            documentation = None
            resources = List.empty
        }