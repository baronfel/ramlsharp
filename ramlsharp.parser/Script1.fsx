#I @"C:\users\chet_husk\documents\github\ramlsharp\ramlsharp.parser\"
#I @"C:\users\chet_husk\documents\github\ramlsharp\ramlsharp\"
#I @"C:\users\chet_husk\documents\github\ramlsharp\ramlsharp.parser\bin\Debug"
#I @"C:\users\chet_husk\documents\github\ramlsharp\packages\Fparsec.1.0.1\lib\net40-client"
#r "FParsec"
#r "FParsecCS"
#r "System.IO"
#r "ramlsharp.model"
#r "ramlsharp.parser"

open FParsec
open ramlsharp.parser

let ramlString  = System.IO.File.ReadAllText(@"C:\Users\chet_husk\Documents\GitHub\ramlsharp\ramlsharp.parser\partial.raml", System.Text.Encoding.UTF8)

run parser.raml ramlString