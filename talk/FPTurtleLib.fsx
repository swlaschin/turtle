(* ======================================
Part of "Thirteen ways of looking at a turtle"
Related blog post: http://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle/
======================================

Common code for FP-style immutable turtle functions.

====================================== *)


// requires Common.fsx to be loaded by parent file
// Uncomment to use this file standalone
#load "Common.fsx"

open System
open Common


// ======================================
// Turtle module
// ======================================

// ======================================
// Turtle module
// ======================================

type TurtleState = {
    position : TurtlePosition
    angle : Angle
    penState : PenState
}

module Turtle = 

    let initialState = {
        position = initialPosition 
        angle = 0.0
        penState = Down
    }                

    // note that state is LAST param in all these functions
    let move distance state =
        Logger.info $"Move %0.1f{distance}"

        let startPosition = state.position 
        let endPosition = calcNewPosition(distance,state.angle,startPosition )

        // draw line if needed
        if state.penState = Down then
            Canvas.drawLine(startPosition,endPosition)

        // return updated state
        {state with position = endPosition}

    let turn angleToTurn state =
        Logger.info $"Turn %0.1f{angleToTurn}"
        
        let newAngle = calcNewAngle(angleToTurn,state.angle) 
        {state with angle = newAngle} // return updated state

    let penUp state =
        Logger.info "Pen up"
        {state with penState = Up} // return updated state

    let penDown state =
        Logger.info "Pen down"
        {state with penState = Down} // return updated state
