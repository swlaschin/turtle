(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #11 - OO-style turtle with dependency rejection

In this design the decisions are kept separate from the I/O actions

More on dependency approaches at https://fsharpforfunandprofit.com/posts/dependencies/
And my talk dependency rejection at https://www.youtube.com/watch?v=P1vES9AgfC4

====================================== *)

#load "Common.fsx"

open System
open Common



// ======================================
// Turtle
// ======================================

type TurtleState = {
    position : TurtlePosition
    angle : float
    penState : PenState
}

type TurtleDecision = {
    logMessage: string
    drawLine: (TurtlePosition * TurtlePosition) option
}

// ======================================
// Turtle class
// ======================================

type Turtle() =

    // internal state
    let mutable position = {x=0.0; y=0.0}
    let mutable angle = 0.0
    let mutable penState = Down

    /// The pure method to do the logic but no I/O
    /// This is easily testable
    member this.MoveDecision(distance) =
        let logMessage = $"Move %0.1f{distance}"

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

        // return the decision
        {logMessage=logMessage; drawLine=drawDecision}

    /// The impure method to do I/O based on the decision
    /// Not easily testable without mocking, but no complicated logic to test anyway!
    member this.Move(distance) =
        
        // do the decision logic
        let decision = this.MoveDecision(distance)

        // do I/O stuff based on the decision
        Logger.info decision.logMessage
        match decision.drawLine with
        | Some (position,newPos) ->
            // decision was to draw a line
            Canvas.drawLine(position,newPos)
        | None ->
            // decision was to do nothing
            () 

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



// -------------------------
// unit test
// -------------------------

let expectIsTrue b msg =
    if not b then Logger.error $"Test failed: {msg}"

let whenPenUpExpectNoLine() =
    let turtle = new Turtle()
    turtle.PenUp()
    let decision = turtle.MoveDecision 50.0  // expect no draw
    expectIsTrue (Option.isNone decision.drawLine) "PenUp"
    
let whenPenDownExpectLine() =
    let turtle = new Turtle()
    turtle.PenDown()
    let decision = turtle.MoveDecision 50.0  // expect no draw
    expectIsTrue (Option.isSome decision.drawLine) "PenDown" 
    

(*
whenPenUpExpectNoLine() 
whenPenDownExpectLine()
*)

// -------------------------
// integration test
// -------------------------

let integrationTest() =
    Canvas.init()
    Canvas.clear()
    
    let turtle = new Turtle()
    turtle.Move 50.0
    turtle.Turn 90.0
    turtle.Move 50.0

(*
integrationTest()
*)