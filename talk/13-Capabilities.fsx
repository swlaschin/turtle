(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/
======================================

API with capabilities

In this design, the turtle exposes a list of functions (capabilities) after each action.
These are the ONLY actions available to the client

More on capability-based security at http://fsharpforfunandprofit.com/posts/capability-based-security/
====================================== *)


#load "Common.fsx"

open System
open Common

// ======================================
// Capability-based Turtle module
// ======================================

/// A private structure representing the turtle
type TurtleState = {
    position : Position
    angle : float
    penState : PenState

    canMove : bool       // new!
}

type TurtleCapabilities = {
    move     : MoveFn option
    turn     : TurnFn
    penUp    : PenUpDownFn
    penDown  : PenUpDownFn
    }
and MoveFn =      Distance -> TurtleCapabilities
and TurnFn =      Angle    -> TurtleCapabilities
and PenUpDownFn = unit     -> TurtleCapabilities

module Turtle =

    let private canNotMove position angle =
        let inline between min max x = x > min && x < max
        let canvasSize = float canvasSize
        // right
        (angle |> between -90. 90. || angle |> between 270. 360.) && position.x >= canvasSize
        // bottom
        || (angle |> between 0. 180.) && position.y <= -canvasSize
        // left
        || (angle |> between 90. 270.) && position.x <= -canvasSize
        // top
        || (angle |> between 180. 360.) && position.y >= canvasSize

    let canMove position angle = canNotMove position angle |> not

    // return distance moved as well as state
    let move distance state =
        Logger.info (sprintf "Move %0.1f" distance)
        // calculate new position
        let startPosition = state.position
        let endPosition,distanceMoved = calcNewPositionBounded(distance,state.angle,startPosition)

        // draw line if needed
        if state.penState = Down then
            Canvas.drawLine(startPosition, endPosition)

        // return new capabilities
        {state with position = endPosition; canMove = canMove endPosition state.angle}


    let turn angleToTurn state =
        Logger.info (sprintf "Turn %0.1f" angleToTurn)
        // calculate new angle
        let newAngle = calcNewAngle(angleToTurn,state.angle )
        // update the state
        {state with angle = newAngle; canMove = canMove state.position newAngle}

    let penUp state =
        Logger.info "Pen up"
        {state with penState = Up}

    let penDown state =
        Logger.info "Pen down"
        {state with penState = Down}

    /// Create the TurtleCapabilities structure associated with a TurtleState
    let rec private createTurtleCapabilities state =
        let ctf = createTurtleCapabilities  // alias

        // create the move function,
        // if the turtle can't move, return None
        let move =
            // the inner function
            let f dist =
                let newState = move dist state
                ctf newState

            // return Some of the inner function
            // if the turtle can move, or None
            if state.canMove then
                Some f
            else
                None

        // create the turn function
        let turn angle =
            let newState = turn angle state
            ctf newState

        // create the pen state functions
        let penDown() =
            let newState = penDown state
            ctf newState

        let penUp() =
            let newState = penUp state
            ctf newState

        // return the structure
        {
        move     = move
        turn     = turn
        penUp    = penUp
        penDown  = penDown
        }

    /// Return the initial turtle.
    /// This is the ONLY public function!
    let start() =
        let state = {
            position = initialPosition
            angle = 0.0
            penState = initialPenState
            canMove = true
        }
        createTurtleCapabilities state




// ======================================
// Maybe
// ======================================

// simplify with Option expression
type MaybeBuilder() =
    member this.Return(x) = Some x
    member this.Bind(x,f) = Option.bind f x
    member this.Zero() = Some()
let maybe = MaybeBuilder()


// ======================================
// Client
// ======================================

open Turtle
let info msg = Logger.info msg
let warn msg = Logger.warn msg

(*
Canvas.init()

// step 1
let turtleCaps = Turtle.start()

// step 2
let turtleCaps = turtleCaps.move.Value 60.0

// step 3
let turtleCaps = turtleCaps.turn 120.0
*)

// test that the boundary is hit
// after second move of 60
let testBoundary() =
    let turtleCaps = Turtle.start()
    match turtleCaps.move with
    | None ->
        warn "Error: Can't do move 1"
    | Some moveFn ->
        let turtleCaps = moveFn 60.0
        match turtleCaps.move with
        | None ->
            warn "Error: Can't do move 2"
        | Some moveFn ->
            let turtleCaps = moveFn 60.0
            match turtleCaps.move with
            | None ->
                warn "Error: Can't do move 3"
            | Some moveFn ->
                info "Success"

/// A function that logs and returns Some(),
/// for use in the "maybe" workflow
let log message =
    printfn "%s" message
    Some ()


let testBoundary2() =
    maybe {
    // create a turtle
    let turtleFns = Turtle.start()

    // attempt to get the "move" function
    let! moveFn = turtleFns.move
    printfn "can move 1"

    // if so, move a distance of 60
    let turtleFns = moveFn 60.0

    // attempt to get the "move" function again
    let! moveFn = turtleFns.move
    printfn "can move 2"

    // if so, move a distance of 60 again
    let turtleFns = moveFn 60.0

    // attempt to get the "move" function again
    let! moveFn = turtleFns.move
    printfn "can move 3"
    } |> ignore


