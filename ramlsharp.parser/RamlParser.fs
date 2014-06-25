namespace ramlsharp.parser

open ramlsharp.model
open System.IO
open System.Net.Http

open FParsec
open AST

module RamlParser =
    let private load text =
        printfn "load text: %s" text
        let result = runParserOnString parser.raml () "meh" text
        match result with
        | Success (raml, _, _) -> printfn "success %A" raml; raml
        | Failure (raml, userstate, endpos) -> 
            printfn "failed. text was %s" text
            failwith "failed reading RAML."

    /// <summary>
    /// This is intended to be the main entry point for the parser.  
    /// You point a file of raml here and we give you back a model of the API described by the string.
    /// </summary>
    /// <param name="path">the local path to the raml file</param>
    let LoadFile path = 
        let contents = File.ReadAllText path
        printfn "contents: %s" contents
        load contents

    /// <summary>
    /// This is intended to be the main entry point for the parser.  
    /// You point a uri of raml here and we give you back a model of the API described by the string.
    /// </summary>
    /// <param name="uri">the network/web path to the raml file</param>
    let LoadUri (uri: string) = 
        let client = new HttpClient()
        let response = client.GetAsync(uri).Result
        let content = response.Content.ReadAsStringAsync().Result
        load content
      
    /// <summary>
    /// You give me a raml definition, and I give you the formatted string output
    /// </summary>
    /// <param name="def"></param>
    let Dump (def:ramlsharp.model.RamlDefinition) =
        System.String.Empty
