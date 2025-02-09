(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #3 - Simple FP - a module of functions with immutable state

In this design, the turtle state is immutable. A module contains functions that return a new turtle state,
and the client uses these turtle functions directly.

The client must keep track of the current state and pass it into the next function call.
Piping is useful here.

====================================== *)


#load "Common.fsx" 

open System
open Common


// ======================================
// Turtle module
// ======================================

type TurtleState = {
    position : TurtlePosition
    angle : Angle
    penState : PenState
}

module Turtle = 

    let initialState = {
        position = initialPosition 
        angle = 0.0
        penState = Down
    }                

    // note that state is LAST param in all these functions
    let move distance state =
        Logger.info $"Move %0.1f{distance}"

        let startPosition = state.position 
        let endPosition = calcNewPosition(distance,state.angle,startPosition )

        // draw line if needed
        if state.penState = Down then
            Canvas.drawLine(startPosition,endPosition)

        // return updated state
        {state with position = endPosition}

    let turn angleToTurn state =
        Logger.info $"Turn %0.1f{angleToTurn}"
        
        let newAngle = calcNewAngle(angleToTurn,state.angle) 
        {state with angle = newAngle} // return updated state

    let penUp state =
        Logger.info "Pen up"
        {state with penState = Up} // return updated state

    let penDown state =
        Logger.info "Pen down"
        {state with penState = Down} // return updated state


// ======================================
// FP Turtle Examples
// ======================================

(*
Canvas.init()
Canvas.clear()

let distance = 100.0
let angle = 120.0
 
// ugly
let s0 = Turtle.initialState
let s1 = Turtle.move distance s0
let s2 = Turtle.turn angle s1

// with piping
Turtle.initialState
|> Turtle.move distance 
|> Turtle.turn angle
|> Turtle.move distance 
|> Turtle.turn angle 
|> Turtle.penUp
|> Turtle.move distance
|> Turtle.move distance
|> Turtle.penDown
|> Turtle.move distance

*)


(*
let drawTriangle() =
    let distance = 100.0
    let angle = 120.0

    let s0 = Turtle.initialState
    let s1 = Turtle.move distance s0
    let s2 = Turtle.turn angle s1
    let s3 = Turtle.move distance s2
    ...
*)

let drawTriangle state =
    let distance = 100.0
    let angle = 120.0
    
    state
    |> Turtle.move distance
    |> Turtle.turn angle
    |> Turtle.move distance
    |> Turtle.turn angle
    |> Turtle.move distance
    |> Turtle.turn angle
    // back home at (0,0) with angle 0

(*
Canvas.init()
Canvas.clear()

Turtle.initialState
|> drawTriangle
|> Turtle.penUp
|> Turtle.move 100.0
|> Turtle.penDown
|> drawTriangle

*)

let drawPolygon n state =
    let distance = 100.0
    let angle = (360.0/float n)
    let angleDegrees = angle * 1.0

    // define a function that draws one side
    let oneSide state sideNumber =
        state
        |> Turtle.move distance
        |> Turtle.turn angleDegrees

    // repeat for all sides
    [1..n] |> List.fold oneSide state

(*
Canvas.init()
Canvas.clear()

Turtle.initialState
|> drawPolygon 4
|> drawPolygon 5
|> drawPolygon 6
*)

