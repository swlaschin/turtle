(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #6b - Turtle Actor -- Posting messages to an Actor

Because the Actor has a message queue, all possible commands are managed with a
single discriminated union type (`TurtleCommand`).

There are no mutables anywhere. The Actor manages the turtle state by
storing the current state as a parameter in the recursive message processing loop.

====================================== *)


#load "Common.fsx" "FPTurtleLib.fsx"

open System
open Common
open FPTurtleLib

// ======================================
// TurtleActor
// ======================================

type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown

type TurtleActor() =

(*
    // here's the pseudo-code
    let rec loop turtleState =
        let command = // read a command from the message queue
        let newState =
            match command with
            | Move distance ->
                Turtle.move distance turtleState
            // etc
        loop newState
*)

    let mailboxProc = MailboxProcessor.Start(fun inbox ->

        let rec loop turtleState = async {
            // read a command message from teh queue
            let! command = inbox.Receive()
            
            // create a new state from executing the command
            let newState =
                match command with
                | Move distance ->
                    Turtle.move distance turtleState
                | Turn angleToTurn ->
                    Turtle.turn angleToTurn turtleState
                | PenUp ->
                    Turtle.penUp turtleState
                | PenDown ->
                    Turtle.penDown turtleState
                    
            return! loop newState
            }

        loop Turtle.initialState )

    // expose the queue externally
    member this.Post(command) =
        mailboxProc.Post command

// ======================================
// Turtle Api Layer
// ======================================



let drawTriangle() =
    let distance = 100.0
    let angle = 120.0
    
    let turtleActor = new TurtleActor()
    turtleActor.Post (Move distance)
    turtleActor.Post (Turn angle)
    turtleActor.Post (Move distance)
    turtleActor.Post (Turn angle)
    turtleActor.Post (Move distance)
    turtleActor.Post (Turn angle)

(*
Canvas.init()
Canvas.clear()
drawTriangle()

Canvas.init()
Canvas.clear()
let turtleActor = new TurtleActor()
turtleActor.Post (Move 50.0)
turtleActor.Post (Turn 120.0)
turtleActor.Post (Move 50.0)
turtleActor.Post (Turn 90.0)

*)