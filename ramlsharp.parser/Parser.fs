module parser

open FParsec
open AST

let ws = spaces
let str s = pstring s

let uri : Parser<Raml,unit> = skipString "baseUri: " >>. restOfLine true |>> BaseUri
let ramlTitle : Parser<Raml,unit> = skipString "title: " >>. restOfLine true |>> Title
let ramlVer : Parser<Raml,unit> = skipString "%#RAML " >>. pfloat |>> Version
let ramlDef = ramlVer // will incrementally expand this definition to include the rest

let raml = ws >>. ramlDef .>> ws .>> eof
