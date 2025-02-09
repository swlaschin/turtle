(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #7 - Event sourcing -- Building state from a list of past events

In this design, the client sends a `Command` to a `CommandHandler`.
The CommandHandler converts that to a list of events and stores them in an `EventStore`.

In order to know how to process a Command, the CommandHandler builds the current state
from scratch using the past events associated with that particular turtle.

Neither the client nor the command handler needs to track state.  Only the EventStore is mutable.

====================================== *)


#load "Common.fsx" "FPTurtleLib.fsx"

open System
open Common
open FPTurtleLib


// ====================================
// Common types for event sourcing
// ====================================

/// A desired action on a turtle
type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown

/// An event representing a state change that actually happened
type TurtleEvent =
    | Moved of Distance * startPos:TurtlePosition * endPos:TurtlePosition
    | Turned of angleTurned:Angle * finalAngle:Angle
    | PenStateChanged of PenState


// ======================================
// Crude in-memory implementation of an EventStore
// ======================================

module EventStore =
    // private mutable data
    let events = ResizeArray<TurtleEvent>()

    let clear() =
        events.Clear()

    /// save an event to storage
    let saveEvent event =
        events.Add event

    /// get all events associated with the specified eventId
    let getEvents() =
        events |> List.ofSeq

    /// for debugging
    let printEvents() =
        for e in getEvents() do
            e |> printfn "%A"

// ====================================
// CommandHandler
// ====================================

module CommandHandler =

    /// Apply an event to the current state and return the new state of the turtle
    let applyEvent state event =
        match event with
        | Moved (distance,startPosition,endPosition) ->
            {state with position = endPosition }
        | Turned (angleTurned,finalAngle) ->
            {state with angle = finalAngle}
        | PenStateChanged penState ->
            {state with penState = penState}

    // Determine what events to generate, based on the command and the state.
    let executeCommand command state =

        match command with
        | Move distance ->
            Logger.info $"Move %0.1f{distance}"

            // calculate new position
            let startPosition = state.position
            let endPosition,boundedResult  = calcNewPositionBounded(distance,state.angle,startPosition)

            // draw line if needed
            if state.penState = Down then
                Canvas.drawLine(startPosition, endPosition)

            // construct the list of events and return them
            if boundedResult.distanceMoved > 0.0 then
                [ Moved (boundedResult.distanceMoved,startPosition,endPosition) ]
            else
                []

        | Turn angleToTurn ->
            Logger.info $"Turn %0.1f{angleToTurn}"
            // calculate new angle
            let newAngle = calcNewAngle(angleToTurn,state.angle) 
            // return list of events
            [ Turned (angleToTurn,newAngle) ]

        | PenUp ->
            Logger.info "Pen up"
            [ PenStateChanged Up ] // return list of events

        | PenDown ->
            Logger.info "Pen down"
            [ PenStateChanged Down ] // return list of events


    /// main function : process a command
    let handleCommand (command:TurtleCommand) =

        /// First load all the events from the event store
        let eventHistory = EventStore.getEvents()

        /// Then, recreate the state before the command
        let stateBeforeCommand =
            eventHistory |> List.fold applyEvent Turtle.initialState

        /// Construct the events arising from the command
        let events = executeCommand command stateBeforeCommand

        // store the events in the event store
        events |> List.iter (EventStore.saveEvent)

    // replay all the events again
    let replayEvents() =
        let eventHistory = EventStore.getEvents()
        for event in eventHistory do
            match event with
            | Moved (distance,startPosition, endPosition) ->
                Canvas.drawLine(startPosition, endPosition)
            | _ ->
                // ignore all other events
                ()
        
// ====================================
// EventSourcing examples
// ====================================

open CommandHandler

let drawTriangle() =
    let distance = 100.0
    let angle = 120.0
    
    handleCommand (Move distance)
    //EventStore.printEvents()
    handleCommand (Turn angle)
    handleCommand (Move distance)
    handleCommand (Turn angle)
    handleCommand (Move distance)
    handleCommand (Turn angle)

(*
Canvas.init()
Canvas.clear()

// draw some figures
EventStore.clear()
drawTriangle()

// see what events have been stored
EventStore.getEvents()

// clear the canvas and replay
Canvas.clear()
CommandHandler.replayEvents()
*)

let drawPolygon n =
    let distance = 100.0
    let angle = (360.0/float n)

    // define a function that draws one side
    let drawOneSide sideNumber =
        handleCommand (Move distance)
        handleCommand (Turn angle)

    // repeat for all sides
    for i in [1..n] do
        drawOneSide i

(*
Canvas.init()
Canvas.clear()

// draw some figures
EventStore.clear()
drawPolygon 4
drawPolygon 5

// see what events have been stored
EventStore.getEvents() 

// clear the canvas and replay
Canvas.clear()
CommandHandler.replayEvents()

*)




