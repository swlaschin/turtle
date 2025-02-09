(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #8 - Stream processing -- Business logic is based on reacting to earlier events

In this design, the "write-side" follows the same pattern as the event-sourcing example.
A client sends a Command to a CommandHandler, which converts that to a list of events and stores them in an EventStore.

However in this design, the CommandHandler only updates state and does NOT do any complex business logic.

The domain logic is done on the "read-side", by listening to events emitted from the event store.

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

/// An event representing a state change that happened
type TurtleEvent =
    | Moved of Distance * startPos:TurtlePosition * endPos:TurtlePosition
    | Turned of angleTurned:Angle * finalAngle:Angle
    | PenStateChanged of PenState


// ======================================
// Crude implementation of an EventStore
// ======================================

module EventStore =
    // private mutable data
    let events = ResizeArray()

    let clear() =
        events.Clear()

    let eventSaved = Event<TurtleEvent>()

    /// save an event to storage
    let saveEvent event =
        events.Add event
        eventSaved.Trigger event

    /// get all events associated with the specified eventId
    let getEvents() =
        events |> List.ofSeq

    /// for debugging
    let printEvents() =
        for e in getEvents() do
            e |> printfn "%A"

// ====================================
// CommandHandler
// (same code as event sourcing example - but no business logic)
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
    // NO logging or canvas
    let executeCommand command state =

        match command with
        | Move distance ->
            let startPosition = state.position
            // calculate new position
            let endPosition,boundedResult = calcNewPositionBounded(distance,state.angle,startPosition)

            // DO NOT ACT ON TURTLE HERE!
            // Compare with EventSourcing code
                            
            // construct the list of events and return them
            if boundedResult.distanceMoved > 0.0 then
                [ Moved (boundedResult.distanceMoved,startPosition,endPosition) ]
            else
                []

        | Turn angleToTurn ->
            // calculate new angle
            let newAngle = calcNewAngle(angleToTurn,state.angle)
            //return list of events
            [ Turned (angleToTurn,newAngle) ]

        | PenUp ->
            [ PenStateChanged Up ]

        | PenDown ->
            [ PenStateChanged Down ]


    /// main function : process a command
    let handleCommand (command:TurtleCommand) =

        /// First load all the events from the event store
        let eventHistory = EventStore.getEvents()

        /// Then, recreate the state before the command
        let stateBeforeCommand =
            eventHistory |> List.fold applyEvent Turtle.initialState

        /// Construct the events from the command and the stateBeforeCommand
        /// Do use the supplied logger for this bit
        let events = executeCommand command stateBeforeCommand

        // store the events in the event store
        events |> List.iter EventStore.saveEvent

// ====================================
// EventProcessors
// ====================================

/// Audit the turtle's actions
let auditingProcessor (eventStream:IObservable<_>) =

    // the function that handles the input from the observable
    let actOnEvent event =
        match event with
        | Moved (distance,startPos,endPos) ->
            printfn $"[AUDIT PROCESSOR] Move %0.1f{distance}"
        | Turned (angleToTurn,endAngle) ->
            printfn $"[AUDIT PROCESSOR] Turn %0.1f{angleToTurn}"
        | PenStateChanged Up ->
            printfn "[AUDIT PROCESSOR] Pen up"
        | PenStateChanged Down ->
            printfn "[AUDIT PROCESSOR] Pen down"

    // start with all events
    eventStream
    // handle these
    |> Observable.subscribe actOnEvent

/// Draw lines on the turtle canvas
let canvasProcessor (eventStream:IObservable<_>) =

    // filter to choose only MovedEvents from TurtleEvents
    let filterEvent = function
        | Moved (dist,startPos,endPos) -> Some (startPos,endPos)
        | _ -> None

    // the function that handles the input from the observable
    let actOnEvent (startPos,endPos) =
        Canvas.drawLine(startPos,endPos)

    // start with all events
    eventStream
    |> Observable.choose filterEvent
    // handle these
    |> Observable.subscribe actOnEvent

/// Listen for "moved" events and aggregate them to keep
let distanceTravelledProcessor (eventStream:IObservable<_>) =

    // filter to choose only MovedEvents from TurtleEvents
    let filterEvent = function
        | Moved (dist,startPos,endPos) -> Some (dist)
        | _ -> None

    // Accumulate the total distance moved so far when a new event happens
    let accumulate distanceSoFar distance =
        distanceSoFar + distance

    // the function that handles the input from the observable
    let actOnEvent distanceSoFar  =
        printfn $"[DISTANCE PROCESSOR] total distance=%0.2f{distanceSoFar}"


    // start with all events
    eventStream
    |> Observable.choose filterEvent
    // accumulate total distance
    |> Observable.scan accumulate 0.0
    // handle these
    |> Observable.subscribe actOnEvent



// ====================================
// StreamProcessing examples
// ====================================

open CommandHandler

let drawTriangle() =
    let distance = 100.0
    let angle = 120.0
    
    // Canvas.init()
    handleCommand (Move distance)
    handleCommand (Turn angle)
    handleCommand (Move distance)
    handleCommand (Turn angle)
    handleCommand (Move distance)
    handleCommand (Turn angle)

let example1() =
    // create an event stream from an IEvent
    let eventStream = EventStore.eventSaved.Publish :> IObservable<_>

    // register the processors
    use auditingProc = auditingProcessor eventStream
    use canvasProc = canvasProcessor eventStream
    use distanceTravelledProc = distanceTravelledProcessor eventStream
    
    // do some commands
    handleCommand (Move 100.0)
    handleCommand (Turn 90.0)
    handleCommand (Move 100.0)

    drawTriangle()
   
    
(*
EventStore.clear()
EventStore.getEvents()

Canvas.init()
Canvas.clear()
Canvas.verbose(false)

example1()

*)