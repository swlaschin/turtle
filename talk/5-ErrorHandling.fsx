(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Wya #5 - FP-style immutable turtle functions WITH ERRORS

The turtle functions return the Result type to indicate errors

More on result handling at https://fsharpforfunandprofit.com/rop/
====================================== *)


#load "Common.fsx" "Result.fsx"

open System
open Common


// ======================================
// Turtle module which returns a Result for commands that could error
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
        penState = initialPenState
    }

    // return distance moved as well as state
    let move distanceRequested state =
        Logger.info $"Move %0.1f{distanceRequested}"
        // calculate new position
        let startPosition = state.position
        let endPosition,bounded = calcNewPositionBounded(distanceRequested,state.angle,startPosition)

        // draw line if needed
        if state.penState = Down then
            Canvas.drawLine(startPosition,endPosition)

        // Return an error if the distance moved is different
        // from that requested
        if bounded.wasBounded then
            Error "Moved out of bounds"
        else
            Ok {state with position = endPosition}

    let turn angleToTurn state =
        if angleToTurn > 360.0 then
            // Return an error if the angle is not allowed
            Error "I'm sorry Dave, I can't turn that much"
        else
            Logger.info $"Turn %0.1f{angleToTurn}"
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
// Error returning Turtle Examples
// ======================================

// example with errors handled explicitly
let errorExample1() =

    Canvas.init()
    Canvas.clear()
    
    let distance = 50.0
    let angle = 120.0
    let log = Logger.info
    
    let s0 = Turtle.initialState
    let result1 = s0 |> Turtle.move distance
    match result1 with
    | Error msg ->
        log $"first move failed -- {msg}"
    | Ok s1 ->
        log "first move succeeded"
        let result2 = Turtle.move distance s1
        match result2 with
        | Error msg ->
            log $"second move failed: {msg}"
            let result3 = Turtle.turn distance s1
            ()
        | Ok s2 ->
            log "second move succeeded"
            ()


open Result

// example with errors handled explicitly
let errorExample2()  =

    let log = Logger.info

    Canvas.init()
    Canvas.clear()
    Canvas.drawBoundary()

    // this one succeeds
    let result1 = result {
        let distance = 50.0
        let angle = 120.0
        
        let s0 = Turtle.initialState
        let! s1 = s0 |> Turtle.move distance
        log "first move succeeded"
        let! s2 = s1 |> Turtle.move distance
        log "second move succeeded"
        let! s3 = s2 |> Turtle.turn angle
        let! s4 = s3 |> Turtle.move distance
        log "third move succeeded"
        return ()
        }

    match result1 with
    | Ok _ ->
        log "every move succeeded"
    | Error msg ->
        log $"Failure: {msg}"


    Canvas.clear()
    Canvas.drawBoundary()

    // this one fails
    let result2 = result {
        let distance = 120.0
        let angle = 120.0
        
        let s0 = Turtle.initialState
        let! s1 = s0 |> Turtle.move distance
        log "first move succeeded"
        let! s2 = s1 |> Turtle.move distance
        log "second move succeeded"
        let! s3 = s2 |> Turtle.turn angle
        let! s4 = s3 |> Turtle.move distance
        log "third move succeeded"
        return ()
        }

    match result2 with
    | Ok _ ->
        log "every move succeeded"
    | Error msg ->
        log $"Failure: {msg}"
