module parser

open FParsec
open AST

let ws = spaces
let str s = pstring s

let baseUri : Parser<Raml,unit> = skipString "baseUri: " >>. restOfLine true |>> BaseUri
let title : Parser<Raml,unit> = skipString "title: " >>. restOfLine true |>> Title
let ramlVer : Parser<Raml,unit> = skipString "%#RAML " >>. pfloat |>> Version
let ramlDef = ramlVer .>> title .>> baseUri // will incrementally expand this definition to include the rest

let raml = ws >>. ramlDef .>> ws .>> eof
