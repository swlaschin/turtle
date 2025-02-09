(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #13 - API with capabilities

In this design, the turtle exposes a list of functions (capabilities) after each action.
These are the ONLY actions available to the client

More on capability-based security at https://fsharpforfunandprofit.com/cap/
====================================== *)


#load "Common.fsx"

open Common

// ======================================
// Capability-based Turtle module
// ======================================

// Turtle state
type TurtleState = {
    position : TurtlePosition
    angle : float
    penState : PenState
    canMove : bool       // new!
}

let initialState = {
    position = initialPosition 
    angle = 0.0
    penState = Down
    canMove = true
}                
 
/// Turtle capabilities -- a set of functions to call
type TurtleCapabilities = {
    move     : MoveFn option  // optional!
    turn     : TurnFn
    penUp    : PenUpDownFn
    penDown  : PenUpDownFn
    }
and MoveFn =      Distance -> TurtleCapabilities
and TurnFn =      Angle    -> TurtleCapabilities
and PenUpDownFn = unit     -> TurtleCapabilities

module Turtle =
    let boundaryRadius = 200.0

    /// Calculate whether the turtle can move forward 
    let private cannotMoveForward position angle =
        let inline between min max x = x >= min && x < max
        // right
        (angle |> between 0. 90. || angle |> between 270. 360.) && position.x >= boundaryRadius 
        // left
        || (angle |> between 90. 270.) && position.x <= -boundaryRadius 
        // top
        || (angle |> between 0. 180.) && position.y >= boundaryRadius 
        // bottom
        || (angle |> between 180. 360.) && position.y <= -boundaryRadius 

    let private canMoveForward position angle =
        // Logger.debug $"position={position} angle={angle} cannotMoveForward={cannotMoveForward position angle}"
        cannotMoveForward position angle |> not

    // return distance moved as well as state
    let private move distance state =
        //^--now private!
        Logger.info $"Move %0.1f{distance}"
        // calculate new position
        let startPosition = state.position
        let endPosition,_ = calcNewPositionBounded(distance,state.angle,startPosition)

        // draw line if needed
        if state.penState = Down then
            Canvas.drawLine(startPosition, endPosition)
       
        // return new capabilities
        {state with position = endPosition; canMove = canMoveForward endPosition state.angle}


    let private turn angleToTurn state =
        //^--now private!
        Logger.info $"Turn %0.1f{angleToTurn}"
        // calculate new angle
        let newAngle = calcNewAngle(angleToTurn,state.angle )
        
        // return new capabilities
        {state with angle = newAngle; canMove = canMoveForward state.position newAngle}

    let private penUp state =
        //^--now private!
        Logger.info "Pen up"
        {state with penState = Up}

    let private penDown state =
        //^--now private!
        Logger.info "Pen down"
        {state with penState = Down}

    /// Create the TurtleCapabilities structure associated with a TurtleState
    let rec private createTurtleCapabilities state =
        let ctf = createTurtleCapabilities  // alias

        // create the move function,
        // if the turtle can't move, return None
        let moveFnOption =
            // the inner function
            let moveFn dist =
                // calculate the state
                let newState = move dist state
                // calculate the capabilities from the new state
                ctf newState

            // return Some of the inner function
            // if the turtle can move, or None
            if state.canMove then
                Some moveFn
            else
                None

        // create the turn function
        let turnFn angle =
            // calculate the state
            let newState = turn angle state
            // calculate the capabilities from the new state
            ctf newState

        // create the pen state functions
        let penDownFn() =
            // calculate the state
            let newState = penDown state
            // calculate the capabilities from the new state
            ctf newState

        let penUpFn() =
            // calculate the state
            let newState = penUp state
            // calculate the capabilities from the new state
            ctf newState

        // return the structure
        {
        move     = moveFnOption
        turn     = turnFn
        penUp    = penUpFn
        penDown  = penDownFn
        }

    /// Return the initial turtle.
    /// This is the ONLY public function!
    let start initialState =
        createTurtleCapabilities initialState

// ======================================
// Examples
// ======================================


let basicExample() = 
    Canvas.init()
    Canvas.clear()
    Canvas.drawBoundary()

    let distance = 100.0
    let angle = 120.0
    
    // get the capabilities
    let mutable turtleCaps = Turtle.start(initialState)

    if turtleCaps.move.IsSome then
        turtleCaps <- turtleCaps.move.Value distance

    turtleCaps <- turtleCaps.turn angle
    
    if turtleCaps.move.IsSome then
        turtleCaps <- turtleCaps.move.Value distance

(*
basicExample()
*)


// test that the boundary is hit eventually
let testBoundaryHit() =
    Canvas.init()
    Canvas.clear()
    Canvas.drawBoundary()

    let distance = 80.0
    let angle = 120.0
   
    let rec drawLineToBoundary moveCount turtleCaps =
        match turtleCaps.move with
        | None ->
            Logger.warn $"Error: Can't move forward on move {moveCount}"
            turtleCaps, moveCount // return caps and moveCount
        | Some moveFn ->
            let turtleCaps = moveFn distance
            drawLineToBoundary (moveCount+1) turtleCaps 

    let turtleCaps, moveCount = Turtle.start initialState |> drawLineToBoundary 1
    
    // reached boundary, so turn
    let turtleCaps2, moveCount2 =
        turtleCaps.turn angle
        |> drawLineToBoundary  moveCount

    // reached boundary, so turn again
    turtleCaps2.turn angle
    |> drawLineToBoundary  moveCount2

(*
testBoundaryHit()
*)

