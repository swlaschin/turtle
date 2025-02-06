(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/
======================================

Way #1: Simple OO -- a class with mutable state

In this design, a simple OO class represents the turtle,
and the client talks to the turtle directly.

====================================== *)

#load "Common.fsx" "OOTurtleLib.fsx"

open System
open Common
open OOTurtleLib

// ======================================
// OO Turtle Examples
// ======================================

(*
Canvas.init()
let distance = 50.0

let turtle = Turtle()
turtle.Move(distance)
turtle.Turn(120.0)
turtle.Move(distance)
turtle.Turn(120.0)
turtle.Move(distance)
turtle.Turn(120.0)

turtle.PenUp()
turtle.Move(distance)
turtle.PenDown()
turtle.Turn(120.0)
*)

let drawTriangle() =
    let distance = 50.0
    let turtle = Turtle()
    turtle.Move(distance)
    turtle.Turn(120.0)
    turtle.Move(distance)
    turtle.Turn(120.0)
    turtle.Move(distance)
    turtle.Turn(120.0)
    // back home at (0,0) with angle 0

(*
Canvas.init()
Canvas.clear()
drawTriangle()
*)

let drawPolygon n =
    let angle = (360.0/float n)
    let angleDegrees = angle * 1.0
    let turtle = Turtle()

    // define a function that draws one side
    let drawOneSide() =
        turtle.Move(50.0)
        turtle.Turn(angleDegrees)

    // repeat for all sides
    for i in [1..n] do
        drawOneSide()

(*
Canvas.init()
Canvas.clear()
drawPolygon(4)
drawPolygon(5)
drawPolygon(6)
*)

