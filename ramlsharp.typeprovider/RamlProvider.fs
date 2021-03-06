﻿namespace ramlsharp.typeprovider

open ramlsharp.parser
open Microsoft.FSharp.Core.CompilerServices
open Samples.FSharp.ProvidedTypes
open System.Reflection
open System.Net.Http
open System.Threading

[<TypeProvider>]
type RamlProvider(config : TypeProviderConfig) as this = 
    inherit TypeProviderForNamespaces()
    let namespaceName = "ramlsharp.model"
    let thisAssembly = Assembly.GetExecutingAssembly()
    let baseType = typeof<AST.RamlDef>
    let staticParams = [ProvidedStaticParameter("uriSource", typeof<string>)] // point us at a URI we can HTTP GET
    
    let ramlType = ProvidedTypeDefinition(thisAssembly, namespaceName, "RamlApi", Some baseType)

    do ramlType.DefineStaticParameters(
        parameters = staticParams,
        instantiationFunction = (fun typeName paramValues ->
            // ok, so given this uri we need to GET the input
            let uri = paramValues.[0].ToString()
            let ramlContent = RamlParser.LoadUri uri

            let constructedType = ProvidedTypeDefinition(thisAssembly, namespaceName, "RamlApi", Some baseType)
            constructedType
        )
    )

    do this.AddNamespace(namespaceName, [ramlType])



[<TypeProviderAssembly>]
do()