(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #4 - State monad - managing state behind the scenes

A "state expression" can be used to hide the details of state management

More on state monad at https://fsharpforfunandprofit.com/monadster/
====================================== *)


#load "Common.fsx" "FPTurtleLibBounded.fsx" "State.fsx"

open System
open Common
open FPTurtleLibBounded
open State


// ======================================
// State monad Turtle Examples
// ======================================

// example with state managed explicitly
let example1() =

    Canvas.init()
    Canvas.clear()
    Canvas.drawBoundary()
    
    let distance = 120.0
    let angle = 120.0
    let log = Logger.info
    
    let s0 = Turtle.initialState
    let bounded1, s1  = s0 |> Turtle.move distance
    if bounded1.wasBounded then
        log "first move failed -- turning"
        let s2 = s1 |> Turtle.turn angle
        let bounded3, s3 = s2 |> Turtle.move distance
        ()
    else
        log "first move succeeded"
        let bounded2, s2  = s1 |> Turtle.move distance
        if bounded2.wasBounded then
            log "second move failed -- turning"
            let s3 = s2 |> Turtle.turn angle
            let bounded4, s4 = s3 |> Turtle.move distance
            ()
        else
            log "second move succeeded"
            let bounded3, s3 = s2 |> Turtle.move distance
            ()

// example with state managed using state expression
let example2() =

    let log = Logger.info
    
    // lift the functions to the State type
    let move x = State (Turtle.move x)
    let turn angle = State (fun state -> (), state |> Turtle.turn angle)

    let distance = 120.0
    let angle = 120.0
    
    // this looks like imperative code but is stateless (and testable) behind the scenes
    let stateExpression = state {
        let! bounded1 = move distance
        if bounded1.wasBounded then
            log "first move failed -- turning"
            do! turn angle
            let! bounded2 = move distance
            return ()
        else
            log "first move succeeded"
            let! bounded2 = move distance
            if bounded2.wasBounded then
                log "second move failed -- turning"
                do! turn angle
                let! bounded3 = move distance
                return ()
            else
                log "second move succeeded"
                let! bounded3 = move distance
                return ()
        }

    // example
    Canvas.init()
    Canvas.clear()
    Canvas.drawBoundary()

    let s0 = Turtle.initialState
    stateExpression |> State.run s0

