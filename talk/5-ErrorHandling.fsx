(* ======================================
Part of "Thirteen ways of looking at a turtle"
Related blog post: http://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle/
======================================

FP-style immutable turtle functions WITH ERRORS

====================================== *)


#load "Common.fsx" "Result.fsx"

open System
open Common


// ======================================
// Turtle module
// ======================================

type TurtleState = {
    position : Position
    angle : Angle
    penState : PenState
}

module Turtle =

    let initialTurtleState = {
        position = initialPosition
        angle = 0.0
        penState = initialPenState
    }

    // return distance moved as well as state
    let move distanceRequested state =
        Logger.info (sprintf "Move %0.1f" distanceRequested)
        // calculate new position
        let startPosition = state.position
        let endPosition,distanceMoved = calcNewPositionBounded(distanceRequested,state.angle,startPosition)

        // draw line if needed
        if state.penState = Down then
            Canvas.drawLine(startPosition,endPosition)

        if distanceMoved <> distanceRequested then
            Error "Moved out of bounds"
        else
            Ok {state with position = endPosition}

    let turn angleToTurn state =
        if angleToTurn > 360.0 then
            Error "I'm sorry Dave, I can't turn that much"
        else
            Logger.info (sprintf "Turn %0.1f" angleToTurn)
            let newAngle = calcNewAngle(angleToTurn,state.angle)
            // update the state
            Ok {state with angle = newAngle}

    let penUp state =
        Logger.info "Pen up"
        Ok {state with penState = Up}

    let penDown state =
        Logger.info "Pen down"
        Ok {state with penState = Down}



// ======================================
// Error Example 1
// ======================================

module ErrorExample1 =

    // example
    Canvas.init()
    let s0 = Turtle.initialTurtleState
    let result1 = s0 |> Turtle.move 80.0
    match result1 with
    | Error msg ->
        log $"first move failed -- {msg}"
    | Ok s1 ->
        log "first move succeeded"
        let result2 = Turtle.move 80.0 s1
        match result2 with
        | Error msg ->
            log $"second move failed: {msg}"
            let result3 = Turtle.turn 120.0 s1
            ()
        | Ok s2 ->
            log "second move succeeded"
            ()

// ======================================
// Error Example 2
// ======================================

open Result

module ErrorExample2  =

    Canvas.init()

    let finalResult = result {
        let s0 = Turtle.initialTurtleState
        let! s1 = s0 |> Turtle.move 80.0
        log "first move succeeded"
        let! s2 = s1 |> Turtle.move 30.0
        log "second move succeeded"
        let! s3 = s2 |> Turtle.turn 120.0
        let! s4 = s3 |> Turtle.move 80.0
        log "third move succeeded"
        return ()
        }

    match finalResult with
    | Ok _ ->
        log "every move succeeded"
    | Error msg ->
        log $"Failure: {msg}"
