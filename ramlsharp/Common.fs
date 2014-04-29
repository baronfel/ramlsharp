namespace Common

module WrappedString =
    /// An interface that all wrapped strings support
    type IWrappedString = 
        abstract Value : string

    /// Create a wrapped value option
    /// 1) canonicalize the input first
    /// 2) If the validation succeeds, return Some of the given constructor
    /// 3) If the validation fails, return None
    /// Null values are never valid.
    let create canonicalize isValid ctor (s:string) = 
        if s = null 
        then None
        else
            let s' = canonicalize s
            if isValid s'
            then Some (ctor s') 
            else None

    /// Apply the given function to the wrapped value
    let apply f (s:IWrappedString) = 
        s.Value |> f 

    /// Get the wrapped value
    let value s = apply id s

    /// Equality test
    let equals left right = 
        (value left) = (value right)
        
    /// Comparison
    let compareTo left right = 
        (value left).CompareTo (value right)

module MarkdownString = 
    open WrappedString
    open FSharp.Markdown

    type T =  MarkdownString of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (MarkdownString s) = this in s

    let create =
        let isValid s =
            let parsed = Markdown.Parse s
            not parsed.Paragraphs.IsEmpty

        WrappedString.create id isValid MarkdownString

    let convert s = WrappedString.apply create s


type HttpMethod = OPTIONS | GET | HEAD | POST | PUT | DELETE | TRACE | CONNECT | PATCH

type MediaType = Xml of string | Json of string// todo: validation per spec

type Include<'a> = {
    path : string
    content : 'a
}

type ItemOrInclude<'a> = 
    Item of 'a // we can just have the thing
    | Include of Include<'a> // we can use a !include to reference some relative path
    | RootReference of string // we can refer to a collection held in a parent or root node by key

type ProtocolType = HTTP | HTTPS

type HttpStatusCode = HttpStatusCode of int // add validation to this to make sure that these are never outside the RFC spec bounds...

type CreateResult<'a> = Success of 'a | Error of string
