namespace Model

module WrappedString =
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

module HttpStatusCode = 
        
    let isInformational n =
        n = 100 || n = 101

    let isSuccessful n =
        seq{200 .. 208}
        |> Seq.append [226]
        |> Seq.exists (fun i -> n = i) 
        

    let isRedirect n =
        seq{300 .. 308} 
        |> Seq.exists (fun i -> n = i)
        

    let isClientError n =
        seq{400..420}
        |> Seq.append (seq{422..426})
        |> Seq.append [429;429;431;440;444;449;450;451]
        |> Seq.append (seq{494..499})
        |> Seq.exists (fun i -> n = i)

    let isServerError n = 
        seq{500..511}
        |> Seq.append (seq{520..524})
        |> Seq.append [598;599]
        |> Seq.exists (fun i -> n = i)

    type T = 
    | Informational of int
    | Successful of int
    | Redirection of int
    | ClientError of int
    | ServerError of int

    let create n = 
        let value = 
            match n with
            | n when isInformational n -> Informational n
            | n when isSuccessful n -> Successful n
            | n when isRedirect n -> Redirection n
            | n when isClientError n -> ClientError n
            | n when isServerError n -> ServerError n
            | n -> failwith "cannot match the provided int into an error code"
        value
    

module Common = 
    type UriString = UriString of string

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

    type CreateResult<'a> = Success of 'a | Error of string
