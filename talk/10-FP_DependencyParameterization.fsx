(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #10 - FP-style turtle with dependency parameterization via partial application

====================================== *)

#load "Common.fsx"

open System
open Common



// ======================================
// Turtle
// ======================================

type TurtleState = {
    position : TurtlePosition
    angle : float
    penState : PenState
}

module Turtle =

    let initialState = {
        position = initialPosition
        angle = 0.0
        penState = initialPenState
    }

    let move logInfo drawLine distance state =
        // ---^-------^ dependencies passed as parameters
        logInfo $"Move %0.1f{distance}"
        let startPosition = state.position
        let endPosition = calcNewPosition(distance,state.angle,startPosition )

        if state.penState = Down then
            drawLine(startPosition,endPosition)

        // update the state
        {state with position = endPosition}

    let turn logInfo angleToTurn state =
        // ---^-- dependencies passed as parameters
        logInfo $"Turn %0.1f{angleToTurn}"
        let newAngle = calcNewAngle(angleToTurn,state.angle)
        {state with angle = newAngle}

    let penUp logInfo state =
        // ---^-- dependencies passed as parameters
        logInfo "Pen up"
        {state with penState = Up}

    let penDown logInfo state =
        // ---^-- dependencies passed as parameters    
        logInfo "Pen down"
        {state with penState = Down}

// ======================================
// implementation of "Interfaces"
// ======================================

let realDrawLine(startPos,endPos) =
    Canvas.drawLine(startPos,endPos)

let mockDrawLine(startPos,endPos) =
    Logger.info $"[MockDrawLine] drawing from ({startPos.x},{startPos.y}) to ({endPos.x},{endPos.y})"

let nullLogger(msg) = () // ignore


// ======================================
// Examples 
// ======================================

let exampleWithRealImplementation() =
    
    // set up functions with real dependencies baked in    
    let move = Turtle.move Logger.info realDrawLine
    // output is new function: "move"
    //    (Distance -> TurtleState -> TurtleState)

    let turn = Turtle.turn Logger.info
    // output is new function: "turn"
    //    (Angle -> TurtleState -> TurtleState)

    Canvas.init()
    Canvas.clear()

    let distance = 100.0
    let angle = 120.0

    Turtle.initialState
    |> move distance
    |> turn angle
    |> move distance
    |> turn angle
    |> move distance
    |> turn angle

(*
exampleWithRealImplementation()
*)

let exampleWithMockImplementation() =
    // set up functions with mock dependencies baked in    
    let move = Turtle.move nullLogger mockDrawLine
    let turn = Turtle.turn nullLogger
    
    let distance = 100.0
    let angle = 120.0

    Turtle.initialState
    |> move distance
    |> turn angle
    |> move distance
    |> turn angle
    |> move distance
    |> turn angle

(*
exampleWithMockImplementation()
*)
