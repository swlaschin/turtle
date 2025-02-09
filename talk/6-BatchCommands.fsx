(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #6 Batch oriented -- Using a list of commands

In this design, the client creates a list of `Command`s that will be intepreted later.
These commands are then run in sequence using the Turtle library functions.

This approach means that there is no state that needs to be persisted between calls by the client.

====================================== *)


#load "Common.fsx" "FPTurtleLib.fsx"

open System
open Common
open FPTurtleLib

// ======================================
// TurtleCommmandHandler
// ======================================

type TurtleCommand =
    | Move of Distance
    | Turn of Angle
    | PenUp
    | PenDown

module TurtleBatch =

    /// Apply a command to the turtle state and return the new state
    let executeCommand state command =
        match command with
        | Move distance ->
            Turtle.move distance state
        | Turn angleToTurn ->
            Turtle.turn angleToTurn state
        | PenUp ->
            Turtle.penUp state
        | PenDown ->
            Turtle.penDown state

    /// Run list of commands in one go
    let run aListOfCommands =
        let mutable state = Turtle.initialState
        for command in aListOfCommands do
           state <- executeCommand state command
        // return final state
        state

    /// Run list of commands in one go
    let run2 aListOfCommands =
        aListOfCommands
        |> List.fold executeCommand Turtle.initialState



// ======================================
// Batch Command Examples
// ======================================

let drawTriangle() =
    // create the list of commands
    let distance = 100.0
    let angle = 120.0
    
    let commands = [
        Move distance
        Turn angle
        Move distance
        Turn angle
        Move distance
        Turn angle
        ]
    // run them
    TurtleBatch.run commands

let example1() =
    Canvas.init()
    Canvas.clear()
    drawTriangle()



