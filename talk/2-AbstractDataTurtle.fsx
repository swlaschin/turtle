(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/
======================================

Abstract Data Turtle - a private type with an associated module of functions

In this design, the details of the turtle structure is hidden from the client,
so the it could be changed without breaking any code.

====================================== *)


#load "Common.fsx"

open System
open Common


// ======================================
// Abstract Data Turtle module
// ======================================


/// A private structure representing the turtle
type TurtleHandle = private {
    mutable position : Position
    mutable angle : Angle
    mutable penState : PenState
}

/// Functions for manipulating a turtle
module Turtle =

    /// return a new TurtleHandle
    let create() = {
        position = initialPosition
        angle = 0.0
        penState = initialPenState
    }


    let move(handle,distance) =
        Logger.info (sprintf "Move %0.1f" distance)

        let newPos = calcNewPosition(distance,handle.angle,handle.position)

        // draw line if needed
        if handle.penState = Down then
            Canvas.drawLine(handle.position,newPos)

        // update the handle
        handle.position <- newPos

    let turn(handle,angleToTurn) =
        Logger.info (sprintf "Turn %0.1f" angleToTurn)

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
// AdtTurtle Client
// ======================================

let drawTriangle() =
    let distance = 50.0
    let handle = Turtle.create()
    Turtle.move(handle, distance)
    Turtle.turn(handle, 120.0)
    Turtle.move(handle, distance)
    Turtle.turn(handle, 120.0)
    Turtle.move(handle, distance)
    Turtle.turn(handle, 120.0)
    // back home at (0,0) with angle 0

(*
Canvas.init()
drawTriangle()
*)

let drawPolygon n =
    let angle = (360.0/float n)
    let angleDegrees = angle * 1.0
    let turtle =  Turtle.create()

    let drawOneSide() =
        Turtle.move(turtle, 50.0)
        Turtle.turn(turtle, angleDegrees)

    // repeat for all sides
    for i in [1..n] do
        drawOneSide()

(*
Canvas.clear()
drawPolygon(4)
drawPolygon(5)
drawPolygon(6)
*)

