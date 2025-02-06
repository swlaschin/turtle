(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/

======================================

FP-style turtle with dependency injection via partial application

====================================== *)

#load "Common.fsx"

open System
open Common



// ======================================
// Turtle
// ======================================

type TurtleState = {
    position : Position
    angle : float
    penState : PenState
}

module Turtle =

    let initialTurtleState = {
        position = initialPosition
        angle = 0.0
        penState = initialPenState
    }

    let move logInfo drawLine distance state =
        logInfo (sprintf "Move %0.1f" distance)
        let startPosition = state.position
        let endPosition = calcNewPosition(distance,state.angle,startPosition )

        if state.penState = Down then
            drawLine(startPosition,endPosition)

        // update the state
        {state with position = endPosition}

    let turn logInfo angleToTurn state =
        logInfo (sprintf "Turn %0.1f" angleToTurn)
        let newAngle = calcNewAngle(angleToTurn,state.angle)
        {state with angle = newAngle}

    let penUp logInfo state =
        logInfo "Pen up"
        {state with penState = Up}

    let penDown logInfo state =
        logInfo "Pen down"
        {state with penState = Down}

let realDrawLine(startPos,endPos) =
    Canvas.drawLine(startPos,endPos)

let mockDrawLine(startPos,endPos) =
    log $"[Mock] drawing from ({startPos.x},{startPos.y}) to ({endPos.x},{endPos.y})"

let nullLogger(msg) = () // ignore


// set up dependencies
let move = Turtle.move Logger.info realDrawLine
// output is new function: "move"
//    (Distance -> TurtleState -> TurtleState)


let turn = Turtle.turn Logger.info
// output is new function: "turn"
//    (Angle -> TurtleState -> TurtleState)

(*
let move = Turtle.move nullLogger mockDrawLine
let turn = Turtle.turn nullLogger
*)

(*
Canvas.init()

Turtle.initialTurtleState
|> move 50.0
|> turn 120.0
|> move 50.0
|> turn 120.0
|> move 50.0
|> turn 120.0
*)