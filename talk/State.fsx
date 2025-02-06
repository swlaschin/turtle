type State<'state,'a> = State of ('state -> 'a * 'state)

module State =

    let run (initialState:'state) (State f) =
        f initialState 

    let retn x = 
        State (fun s -> x,s)

    let bind f (xState:State<'state,'a>) :State<'state,'b> = 
        let innerFn state =
            let x1,state1 = xState |> run state 
            let x2,state2 = (f x1) |> run state1
            x2,state2
        State innerFn

type StateBuilder() =
    member this.Return(x) = 
        State.retn x
    member this.Bind(m,f) = 
        State.bind f m
    member this.ReturnFrom(m) = 
        m
    member this.Zero() = 
        this.Return ()

let state = StateBuilder()