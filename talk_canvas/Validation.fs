namespace global

type Validation<'Success,'Failure> =
    Result<'Success,'Failure list>

module Validation =

    let map f (x:Validation<_,_>) :Validation<_,_> =
        Result.map f x

    let bind f (x:Validation<_,_>) :Validation<_,_> =
        Result.bind f x

    /// Apply a Validation<fn> to a Validation<x> applicatively
    let apply (fV:Validation<_,_>) (xV:Validation<_,_>) :Validation<_,_> =
        match fV, xV with
        | Ok f, Ok x -> Ok (f x)
        | Error errs1, Ok _ -> Error errs1
        | Ok _, Error errs2 -> Error errs2
        | Error errs1, Error errs2 -> Error (errs1 @ errs2)

[<AutoOpen>]
module ValidationComputationExpression =

    type ValidationBuilder() =
        member __.Return(x) =
            Ok x
        member __.Bind(x, f) =
            Validation.bind f x
        member __.ReturnFrom(x) =
            x
        member this.Zero() =
            this.Return ()
        member __.Delay(f) =
            f
        member __.Run(f) =
            f()
        member __.MergeSources(x1, x2) =
            let (<!>) = Validation.map
            let (<*>) = Validation.apply
            (fun x y -> (x,y)) <!> x1 <*> x2

        member this.Combine (a,b) =
            this.Bind(a, fun () -> b())

    let validation = new ValidationBuilder()

(*
// test
module Test =
    type T = Validation<int,string>
    
    let x1orError :T = Ok 1
    let x2orError :T = Ok 2
    let x3orError :T = Error ["x3 bad"]
    let x4orError :T = Error ["x4 bad"]
    
    let result1 = validation {
        let! x1 = x1orError
        and! x2 = x2orError
        return (x1 + x2)
    }
    
    let result2 = validation {
        let! x1 = x1orError
        and! x3 = x3orError
        return (x1 + x3)
    }
    
    let result3 = validation {
        let! x4 = x4orError
        and! x3 = x3orError
        return (x4 + x3)
    }
*)    