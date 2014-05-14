namespace ramlsharp.model

type StringParams = {
    enum : string list option // if this is defined, then input value must be in this list. if not in list, ERROR,
    pattern : string option // Perl 5 regex string
    minLength : int64 option
    maxLength : int64 option
}

type NumberParams = {
    minimum : int64 option
    maximum : int64 option
}

type CommonParams = {
    example : string option
    repeat : bool option
    required : bool option // for URI parameters, this needs to default to true, rather than none
    propDefault : string option // name is actually 'default'. does not mean 'client, send this'. means 'if you don't send, server assumes this'
}

type ParameterType = 
    String of (CommonParams option * StringParams option) //
    | Number of (CommonParams option * NumberParams option) // including YAML-defined floating points
    | Integer of (CommonParams option * NumberParams option) // must be an integer, no floats, strict subset of Number
    | Date of CommonParams option // RFC2616 date string, in UTC/GMT: Sun, 06 Nov 1994 08:49:37 GMT
    | Boolean of CommonParams option// true or false only are accepted
    | File of CommonParams option// ???

type NamedParameter = {
    displayName : string option // defaults to the key (the name of the property itself)
    name : string
    description : MarkdownString option
    paramType : ParameterType option // default to string if None
}

type UriParameter = NamedParameter
