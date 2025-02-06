// ======================================
// Result type and companion module
// ======================================

open System

/// Computation Expression
type ResultBuilder() =
    member __.Return(x) = Ok x
    member __.Bind(x, f) = Result.bind f x

    member __.ReturnFrom(x) = x
    member this.Zero() = this.Return ()

    member __.Delay(f) = f
    member __.Run(f) = f()
        
let result = ResultBuilder()
