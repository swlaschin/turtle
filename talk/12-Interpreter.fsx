(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/
======================================

The interpreter pattern

In this design, the client builds a data structure (`TurtleProgram`) that represents the instructions.

This Turtle Program can then interpreted later in various ways

====================================== *)


open System

#load "Common.fsx" "FPTurtleLibBounded.fsx"

open System
open Common
open FPTurtleLibBounded


open Turtle

/// Create a union type to represent each instruction
type TurtleProgram<'a> =
    //           (input params)  (callback)
    | Stop     of 'a
    | Move     of Distance    * (Distance -> TurtleProgram<'a>)
    | Turn     of Angle       * (unit -> TurtleProgram<'a>)
    | PenUp    of (* none *)    (unit -> TurtleProgram<'a>)
    | PenDown  of (* none *)    (unit -> TurtleProgram<'a>)

// ------------------------
// Interpreters for Turtle Program
// ------------------------

/// Interpret by calling the turtle functions
let rec interpretAsTurtle state program =
    match program  with
    | Stop _ ->
        state

    | Move (dist,next) ->
        // act on the turtle
        let actualDistance,newState = Turtle.move dist state

        // do the next step
        let nextProgram = next actualDistance // compute the next step
        interpretAsTurtle newState nextProgram

    | Turn (angle,next) ->
        // act on the turtle
        let newState = Turtle.turn angle state

        // do the next step
        let nextProgram = next()
        interpretAsTurtle newState nextProgram

    | PenUp next ->
        // act on the turtle
        let newState = Turtle.penUp state

        // do the next step
        let nextProgram = next()
        interpretAsTurtle newState nextProgram

    | PenDown next ->
        // act on the turtle
        let newState = Turtle.penDown state

        // do the next step
        let nextProgram = next()
        interpretAsTurtle newState nextProgram

/// Interpret by accumulating distance
let rec interpretAsDistance distanceSoFar program =

    match program with
    | Stop _ ->
        distanceSoFar

    | Move (dist,next) ->
        // calculate the distance
        let newDistanceSoFar = distanceSoFar + dist

        // do the next step
        let nextProgram = next dist
        interpretAsDistance newDistanceSoFar nextProgram

    | Turn (angle,next) ->
        // no change in distanceSoFar

        // do the next step
        let nextProgram = next()
        interpretAsDistance distanceSoFar nextProgram

    | PenUp next ->
        // no change in distanceSoFar

        // do the next step
        let nextProgram = next()
        interpretAsDistance distanceSoFar nextProgram

    | PenDown next ->
        // no change in distanceSoFar
        // do the next step
        let nextProgram = next()
        interpretAsDistance distanceSoFar nextProgram

// ============================================================================
// Turtle Program computation expression
//  * changed type to be generic so that bind works properly
// ============================================================================

module TurtleProgramWorkflow =

    open Turtle

    let returnT x =
        Stop x

    let rec bindT f inst  =
        match inst with
        | Stop x ->
            f x
        | Move(dist,next) ->
            Move(dist,next >> bindT f)
        | Turn(angle,next) ->
            Turn(angle,next >> bindT f)
        | PenUp(next) ->
            PenUp(next >> bindT f)
        | PenDown(next) ->
            PenDown(next >> bindT f)

    // define a computation expression builder
    type TurtleProgramBuilder() =
        member this.Return(x) = returnT x
        member this.Bind(x,f) = bindT f x
        member this.Zero(x) = returnT ()

// create an instance of the computation expression builder
let turtleProgram = TurtleProgramWorkflow.TurtleProgramBuilder()



// ------------------------
// Example of Turtle Program without computation expression
// ------------------------

let drawTriangle =
    Move (50.0, fun actualDistance ->
    if actualDistance < 50.0 then
        printfn "error"
        Turn (120.0, fun () ->
        Move (50.0, fun actualDistance ->
        Stop ()
        ))
    else
        Turn (120.0, fun () ->
        Move (50.0, fun actualDistance ->
        Turn (120.0, fun () ->
        Move (50.0, fun actualDistance ->
        Turn (120.0, fun () ->
        Stop () 
        )))))
     )

// val drawTriangle : TurtleProgram

// ------------------------
// Example of Turtle Program with computation expression
// ------------------------

// helper functions
open TurtleProgramWorkflow
let stop = fun x -> Stop x
let move dist  = Move (dist, stop)
let turn angle  = Turn (angle, stop)
let penUp  = PenUp stop
let penDown  = PenDown stop

(*
module ComputationExpressionExample =

    let drawTriangle = turtleProgram {
        let! actualDist1 = move 50.0
        if actualDistance < 50.0 then
            printfn "error"
            do! turn 120.0
            let! actualDist2 = move 50.0
            do! turn 120.0
        else
            do! turn 120.0
            let! actualDist2 = move 50.0
            do! turn 120.0
            let! actualDist3 = move 50.0
            do! turn 120.0
        }

*)


// ------------------------
// Example: TurtleProgram interpret as displayed turtle
// ------------------------

(*
let program = drawTriangle
let interpret = interpretAsTurtle   // choose an interpreter
let initialState = Turtle.initialTurtleState

// run interpreter
Canvas.init()
interpret initialState program |> ignore

*)

// output
(*
Move 50.0
Turn 120.0
Move 50.0
Turn 120.0
Move 50.0
Turn 120.0
*)

// ------------------------
// Example: TurtleProgram interpret as distance
// ------------------------

(*
let program = drawTriangle           // same program
let interpret = interpretAsDistance  // choose an interpreter

// run interpreter
let initialState = 0.0
interpret initialState program 
|> printfn "Total distance moved is %0.1f"
*)

// output
(*
Total distance moved is 150.0
*)

