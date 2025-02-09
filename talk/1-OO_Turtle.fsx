(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #1: Simple OO -- a class with mutable state

In this design, a simple OO class represents the turtle,
and the client talks to the turtle directly.

====================================== *)

#load "Common.fsx" 

open System
open Common

// ======================================
// Turtle class
// ======================================

type Turtle() =

    // internal mutable state
    let mutable position = initialPosition 
    let mutable angle = 0.0
    let mutable penState = Down
    
    // methods    
    member this.Move(distance) =
        Logger.info $"Move %0.1f{distance}"

        // do calculation
        let newPos = calcNewPosition(distance,angle,position)

        // draw line if needed
        if penState = Down then
            Canvas.drawLine(position,newPos)

        // update the state
        position <- newPos 


    member this.Turn(angleToTurn) =
        Logger.info $"Turn %0.1f{angleToTurn}"

        // do calculation
        let newAngle = calcNewAngle(angleToTurn,angle) 
        
        // update the state
        angle <- newAngle 

    member this.PenUp() =
        Logger.info "Pen up" 
        penState <- Up

    member this.PenDown() =
        Logger.info "Pen down" 
        penState <- Down


// ======================================
// OO Turtle Examples
// ======================================

(*
Canvas.init()
Canvas.clear()

let distance = 100.0
let angle = 120.0

let turtle = Turtle()
turtle.Move(distance)
turtle.Turn(angle)
turtle.Move(distance)
turtle.Turn(angle)
turtle.Move(distance)
turtle.Turn(angle)

turtle.PenUp()
turtle.Move(distance)
turtle.PenDown()
turtle.Turn(120.0)
*)

let drawTriangle(turtle:Turtle) =
    let distance = 100.0
    let angle = 120.0
    
    turtle.Move(distance)
    turtle.Turn(angle)
    turtle.Move(distance)
    turtle.Turn(angle)
    turtle.Move(distance)
    turtle.Turn(angle)
    // back home at (0,0) with angle 0

(*
Canvas.init() 
Canvas.clear()

let turtle = Turtle()
drawTriangle(turtle)
*)

let drawPolygon n =
    let distance = 100.0
    let angle = (360.0/float n)
    let angleDegrees = angle * 1.0
    let turtle = Turtle()

    // define a function that draws one side
    let drawOneSide() =
        turtle.Move(distance)
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

