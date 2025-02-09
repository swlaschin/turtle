(* ======================================
Part of "Thirteen ways of looking at a turtle"
Related blog post: http://fsharpforfunandprofit.com/posts/13-ways-of-looking-at-a-turtle/
======================================

Common code for FP-style immutable turtle functions WITH BOUNDS

====================================== *)


// requires Common.fsx to be loaded by parent file
// Uncomment to use this file standalone
#load "Common.fsx"

open System
open Common

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
        penState = initialPenState
    }                

    // return distance moved as well as state
    let move distance state =
        Logger.info $"Move %0.1f{distance}"
        
        // calculate new position 
        let startPosition = state.position
        let endPosition,bounded = calcNewPositionBounded(distance,state.angle,startPosition)
        
        if state.penState = Down then
            Canvas.drawLine(startPosition, endPosition)

        // return new state, distanceMoved and whether it was bounded
        bounded, {state with position = endPosition}


    let turn angleToTurn state =
        Logger.info $"Turn %0.1f{angleToTurn}"
        let newAngle = calcNewAngle(angleToTurn,state.angle ) 
        // update the state
        {state with angle = newAngle}

    let penUp state =
        Logger.info "Pen up" 
        {state with penState = Up}

    let penDown state =
        Logger.info "Pen down" 
        {state with penState = Down}


