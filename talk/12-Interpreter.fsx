(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #12 - The interpreter pattern

In this design, the client builds a data structure (`TurtleProgram`) that represents the instructions.

This Turtle Program can then be interpreted later in various ways

====================================== *)


open System

#load "Common.fsx" "FPTurtleLibBounded.fsx"

open System
open Common
open FPTurtleLibBounded


/// Create a union type to represent each instruction
type TurtleProgram<'a> =
    //           (input params)  (callback)
    | Stop     of 'a
    | Move     of Distance    * (BoundedResult -> TurtleProgram<'a>)
    | Turn     of Angle       * (unit -> TurtleProgram<'a>)
    | PenUp    of (* none *)    (unit -> TurtleProgram<'a>)
    | PenDown  of (* none *)    (unit -> TurtleProgram<'a>)


// ============================================================================
// Turtle Program computation expression
//  * changed type to be generic so that bind works properly
// ============================================================================

module TurtleProgramExpression =

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
let turtleProgram = TurtleProgramExpression.TurtleProgramBuilder()


// ------------------------
// Example of Turtle Program without computation expression
// ------------------------

let drawTriangleExplicit =
    let distance = 100.0
    let angle = 120.0
    
    Move (distance, fun boundedResult ->
    if boundedResult.wasBounded then
        // hit wall, so turn
        printfn "hit wall"
        Turn (angle, fun () ->
        Move (distance, fun boundedResult ->
            if boundedResult.wasBounded then
                printfn "hit wall"
                Stop ()
            else
                Stop ()
        ))
    else
        // keep going straight
        Move (distance, fun boundedResult ->
        Turn (angle, fun () ->
        Move (distance, fun boundedResult ->
        Turn (angle, fun () ->
        Stop () 
        ))))
     )

// val drawTriangleExplicit : TurtleProgram

// ------------------------
// Example of Turtle Program with computation expression
// ------------------------

// helper functions
let stop = Stop 
let move dist  = Move (dist, stop)
let turn angle  = Turn (angle, stop)
let penUp  = PenUp stop
let penDown  = PenDown stop

let drawStraightLineToBoundary = turtleProgram {
    let distance = 150.0
    
    let! boundedResult = move distance
    if boundedResult.wasBounded then
        // hit wall, so stop
        return()
    else
        // keep going straight
        let! boundedResult = move distance
        if boundedResult.wasBounded then
            // hit wall, so turn
            printfn "hit wall"
            return()
        else
            // keep going straight
            let! boundedResult = move distance
            return()
    }

let drawLotsOfLinesExpression = turtleProgram {
    let angle = 120.0

    do! drawStraightLineToBoundary
    // hit wall, so turn
    do! turn angle
    do! drawStraightLineToBoundary
    do! turn angle
    do! drawStraightLineToBoundary
    do! turn -angle
    do! drawStraightLineToBoundary
    do! turn -angle
    do! drawStraightLineToBoundary
    }


// ====================================================
// Interpreters for Turtle Program
// ====================================================

//--------------------------------------------------
// Interpret by calling the turtle functions
//--------------------------------------------------

let rec interpretAsTurtle state program =
    match program  with
    | Stop _ ->
        state

    | Move (dist,next) ->
        // act on the turtle
        let boundedResult,newState = Turtle.move dist state

        // do the next step
        let nextProgram = next boundedResult // compute the next step
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

//--------------------------------------------------
// Interpret by accumulating distance
//--------------------------------------------------

let rec interpretAsDistance distanceSoFar program =

    match program with
    | Stop _ ->
        distanceSoFar

    | Move (dist,next) ->
        // calculate the distance
        let newDistanceSoFar = distanceSoFar + dist
        
        // create a dummy result
        let boundedResult = {distanceMoved=newDistanceSoFar; wasBounded=false}

        // do the next step
        let nextProgram = next boundedResult
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


// ------------------------
// Example: TurtleProgram interpret as displayed turtle
// ------------------------

let exampleInterpretAsTurtle() =
    
    let program = drawLotsOfLinesExpression
    let interpret = interpretAsTurtle   // choose an interpreter
    let initialState = Turtle.initialState

    // run interpreter
    Canvas.init()
    Canvas.clear()
    Canvas.drawBoundary()
    
    interpret initialState program |> ignore

(*
exampleInterpretAsTurtle()
*)    

// output
(*
INFO Move 150.0
INFO Move 150.0
hit wall
INFO Turn 120.0
INFO Move 150.0
INFO Move 150.0
hit wall
INFO Turn 120.0
INFO Move 150.0
etc
*)

// ------------------------
// Example: TurtleProgram interpret as distance
// ------------------------

let exampleInterpretAsDistance() =
    let program = drawLotsOfLinesExpression // same program
    let interpret = interpretAsDistance  // choose an interpreter

    // run interpreter
    let initialState = 0.0
    interpret initialState program 
    |> printfn "Total distance moved is %0.1f"


(*
exampleInterpretAsDistance()
*)    

// output
(*
Total distance moved is 2250.0
*)

