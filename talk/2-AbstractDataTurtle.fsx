(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #2: Abstract Data Turtle - a private type with an associated module of functions

In this design, the details of the turtle state is hidden from the client,
so that it could be changed without breaking any code.

Unlike OO, the state is separate from the functions that act on it.
This is useful because it forces you to avoid inheritance and use composition instead.

====================================== *)


#load "Common.fsx"

open System
open Common


// ======================================
// AbstractDataTurtle module
// ======================================

/// Functions for manipulating a turtle
module Turtle =

    /// A private structure representing the turtle
    type TurtleHandle = private {
        mutable position : TurtlePosition
        mutable angle : Angle
        mutable penState : PenState
    }

    /// return a new TurtleHandle
    let create() = {
        position = initialPosition
        angle = 0.0
        penState = initialPenState
    }


    let move(handle,distance) =
        Logger.info $"Move %0.1f{distance}"

        let newPos = calcNewPosition(distance,handle.angle,handle.position)

        // draw line if needed
        if handle.penState = Down then
            Canvas.drawLine(handle.position,newPos)

        // update the handle
        handle.position <- newPos

    let turn(handle,angleToTurn) =
        Logger.info $"Turn %0.1f{angleToTurn}"

        // calculate new angle
        let newAngle = calcNewAngle(angleToTurn,handle.angle)

        // update the handle
        handle.angle <- newAngle

    let penUp(handle) =
        Logger.info "Pen up"
        handle.penState <- Up

    let penDown(handle) =
        Logger.info "Pen down"
        handle.penState <- Down

// ======================================
// AbstractDataTurtle examples
// ======================================

(*
Canvas.init()
Canvas.clear()

let distance = 100.0
let angle = 120.0

let handle = Turtle.create()
Turtle.move(handle, distance)
Turtle.turn(handle, angle)

Turtle.penUp(handle)
Turtle.move(handle, distance)
Turtle.penDown(handle)
Turtle.move(handle, distance)
*)

// Like the library functions, this takes a handle as parameter
// (no need for special extension methods)
let drawTriangle handle =
    let distance = 100.0
    let angle = 120.0
    
    Turtle.move(handle, distance)
    Turtle.turn(handle, angle)
    Turtle.move(handle, distance)
    Turtle.turn(handle, angle)
    Turtle.move(handle, distance)
    Turtle.turn(handle, angle)
    // back home at (0,0) with angle 0

    // return handle for piping
    handle        


(*
Canvas.init()
Canvas.clear()

let handle = Turtle.create()
drawTriangle handle

Turtle.move(handle, 100)
drawTriangle handle
*)

// Like the library functions, this takes a handle as parameter
// (no need for special extension methods)
let drawPolygon n handle =
    let distance = 100.0
    let angle = (360.0/float n)
    let angleDegrees = angle * 1.0

    let drawOneSide() =
        Turtle.move(handle, distance)
        Turtle.turn(handle, angleDegrees)

    // repeat for all sides
    for i in [1..n] do
        drawOneSide()

    // return handle for piping
    handle        

(*
Canvas.init()
Canvas.clear()

let handle = Turtle.create()

handle 
|> drawPolygon 4 
|> drawPolygon 5 
|> drawPolygon 6
*)

