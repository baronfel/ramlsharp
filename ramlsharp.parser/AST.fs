module AST

type ParameterType = 
    | String
    | Number

type Raml = 
    | Version of float
    | Title of string
    | BaseUri of string // whole uri
    | Parameter of string * ParameterType option // parameter name, then parameter type and value
