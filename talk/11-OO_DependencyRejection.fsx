(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/

======================================

FP-style turtle with dependency rejection

====================================== *)

#load "Common.fsx"

open System
open Common



// ======================================
// Turtle
// ======================================

type TurtleState = {
    position : Position
    angle : float
    penState : PenState
}

type TurtleDecision = {
    logMessage: string
    draw: (Position * Position) option
}

// ======================================
// Turtle class
// ======================================

type Turtle() =

    // internal state
    let mutable position = {x=0.0; y=0.0}
    let mutable angle = 0.0
    let mutable penState = Down

    // pure method
    member this.MoveDecision(distance) =
        let logMessage = sprintf "Move %0.1f" distance

        // do calculation
        let newPos = calcNewPosition(distance,angle,position)

        // draw line if needed
        let drawDecision =
            if penState = Down then
                Some(position,newPos)
            else
                None

        // update the state
        position <- newPos

        {logMessage=logMessage; draw=drawDecision}

    // impure method
    member this.Move(distance) =
        // decision
        let decision = this.MoveDecision(distance)

        // I/O stuff
        Logger.info decision.logMessage
        match decision.draw with
        | Some (position,newPos) ->
            Canvas.drawLine(position,newPos)
        | None ->
            () // do nothing

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

// -------------------------
// unit test
// -------------------------

let turtle = new Turtle()
turtle.PenUp()
turtle.MoveDecision 50.0  // expect no draw
turtle.PenDown()
turtle.MoveDecision 50.0  // expect draw
turtle.Turn 90.0
turtle.MoveDecision 50.0  // expect draw

// -------------------------
// integration test
// -------------------------

Canvas.init()
let turtle = new Turtle()
turtle.Move 50.0
turtle.Turn 90.0
turtle.Move 50.0

*)