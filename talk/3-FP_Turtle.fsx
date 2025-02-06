(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/
======================================

Simple FP - a module of functions with immutable state

In this design, the turtle state is immutable. A module contains functions that return a new turtle state,
and the client uses these turtle functions directly.

The client must keep track of the current state and pass it into the next function call.

====================================== *)


#load "Common.fsx" "FPTurtleLib.fsx"

open System
open Common
open FPTurtleLib

// ======================================
// FP Turtle Client
// ======================================


(*
let drawTriangle() =
    let s0 = Turtle.initialTurtleState
    let s1 = Turtle.move 50.0 s0
    let s2 = Turtle.turn 120.0 s1
    let s3 = Turtle.move 50.0 s2
    ...
*)

let drawTriangle() =
    Turtle.initialTurtleState
    |> Turtle.move 50.0
    |> Turtle.turn 120.0
    |> Turtle.move 50.0
    |> Turtle.turn 120.0
    |> Turtle.move 50.0
    |> Turtle.turn 120.0
    // back home at (0,0) with angle 0

    (*
    Canvas.init()
    drawTriangle()
    *)

let drawPolygon n =
    let angle = (360.0/float n)
    let angleDegrees = angle * 1.0

    // define a function that draws one side
    let oneSide state sideNumber =
        state
        |> Turtle.move 50.0
        |> Turtle.turn angleDegrees

    // repeat for all sides
    [1..n]
    |> List.fold oneSide Turtle.initialTurtleState

(*
Canvas.init()
drawPolygon 4
drawPolygon 5
drawPolygon 6
*)

