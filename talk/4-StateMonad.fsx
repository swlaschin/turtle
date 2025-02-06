(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/
======================================

State monad - managing state behind the scenes


====================================== *)


#load "Common.fsx" "FPTurtleLibBounded.fsx" "State.fsx"

open System
open Common
open FPTurtleLibBounded
open State


// ======================================
// FP Turtle Client
// ======================================

module FPTurtleState1 =

    // example
    Canvas.init()
    let s0 = Turtle.initialTurtleState
    let dist1,s1 = s0 |> Turtle.move 80.0
    if dist1 < 80.0 then
        log "first move failed -- turning"
        let s2 = s1 |> Turtle.turn 120.0
        let dist3,s3 = s2 |> Turtle.move 80.0
        ()
    else
        log "first move succeeded"
        let dist2,s2 = s1 |> Turtle.move 80.0
        if dist2 < 80.0 then
            log "second move failed -- turning"
            let s3 = s2 |> Turtle.turn 120.0
            let dist4,s4 = s3 |> Turtle.move 80.0
            ()
        else
            log "second move succeeded"
            let dist3,s3 = s2 |> Turtle.move 50.0
            ()

module FPTurtleState2 =

    // lift the functions to the State type
    let move x = State (Turtle.move x)
    let turn angle = State (fun state -> (), state |> Turtle.turn angle)

    let stateExpression = state {

        let! dist1 = move 80.0
        if dist1 < 80.0 then
            log "first move failed -- turning"
            do! turn 120.0
            let! dist3 = move 80.0
            return ()
        else
            log "first move succeeded"
            let! dist2 = move 80.0
            if dist2 < 80.0 then
                log "second move failed -- turning"
                do! turn 120.0
                let! dist4 = move 80.0
                return ()
            else
                log "second move succeeded"
                let! dist3 = move 50.0
                return ()
        }

    // example
    Canvas.init()
    let s0 = Turtle.initialTurtleState
    stateExpression |> State.run s0
