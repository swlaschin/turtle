(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/

======================================

Common code for OO-style mutable turtle class

====================================== *)

// requires Common.fsx to be loaded by parent file
// Uncomment to use this file standalone
#load "Common.fsx"

open System
open Common

// ======================================
// Turtle class
// ======================================

type Turtle() =

    // internal state
    let mutable position = {x=0.0; y=0.0} 
    let mutable angle = 0.0
    let mutable penState = Down
    
    // methods    
    member this.Move(distance) =
        Logger.info (sprintf "Move %0.1f" distance)

        // do calculation
        let newPos = calcNewPosition(distance,angle,position)

        // draw line if needed
        if penState = Down then
            Canvas.drawLine(position,newPos)

        // update the state
        position <- newPos 


    member this.Turn(angleToTurn) =
        Logger.info (sprintf "Turn %0.1f" angleToTurn)

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


(*
// test

Canvas.init()
let turtle = new Turtle()
turtle.Move 20.0 
turtle.Turn 90.0
turtle.Move 20.0 
turtle.Turn 90.0
turtle.Move 20.0 
turtle.Turn 90.0
turtle.Move 20.0 

*)