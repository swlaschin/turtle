(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: https://fsharpforfunandprofit.com/turtle/
======================================

Way #9 - OO-style turtle class with dependency injection

====================================== *)

#load "Common.fsx"

open System
open Common


// ======================================
// Interfaces
// ======================================

type ILogger =
    abstract Info : string -> unit
    abstract Error : string -> unit

type ICanvas =
    abstract DrawLine : TurtlePosition * TurtlePosition -> unit
    abstract Clear : unit -> unit

type ITurtle =
    abstract Move : Distance -> unit
    abstract Turn : Angle -> unit
    abstract PenUp : unit -> unit
    abstract PenDown : unit -> unit

// ======================================
// Turtle class
// ======================================

type Turtle(logger:ILogger, canvas:ICanvas) as this =
    //------^ constructor injection

    let mutable position = initialPosition
    let mutable angle = 0.0
    let mutable penState = initialPenState

    member this.Move(distance) =
        logger.Info $"Move %0.1f{distance}"
        let newPos = calcNewPosition(distance,angle,position)
        if penState = Down then
            canvas.DrawLine(position,newPos)

        // update the state
        position <- newPos

    member this.Turn(angleToTurn) =
        logger.Info $"Turn %0.1f{angleToTurn}"
        let newAngle = calcNewAngle(angleToTurn,angle)
        // update the state
        angle <- newAngle

    member this.PenUp() =
        logger.Info "Pen up"
        penState <- Up

    member this.PenDown() =
        logger.Info "Pen down"
        penState <- Down

    // implement the ITurtle interface
    interface ITurtle with
        member _.Move(distance) = this.Move(distance)
        member _.Turn(angleToTurn) = this.Turn(angleToTurn)
        member _.PenUp() = this.PenUp()
        member _.PenDown() = this.PenDown()



// ======================================
// Interface implementations
// ======================================

let logger =
    {new ILogger with
        member _.Info(msg) = Logger.info(msg)
        member _.Error(msg) = Logger.error(msg)
        }

let realCanvas =
    {new ICanvas with
        member _.DrawLine(startPos,endPos) =
            Canvas.drawLine(startPos,endPos)
        member _.Clear() =
            Canvas.clear()
        }

let mockCanvas =
    {new ICanvas with
        member _.DrawLine(startPos,endPos) =
            Logger.info $"[MockCanvas] drawing from ({startPos.x},{startPos.y}) to ({endPos.x},{endPos.y})"
        member _.Clear() =
            () // do nothing
        }

let mockTurtle() : ITurtle =
    let log = Logger.info
    {new ITurtle with
        member _.Move(distance) = log $"[MockTurtle] Move {distance}"
        member _.Turn(angleToTurn) = log $"[MockTurtle] Turn {angleToTurn}"
        member _.PenUp() = log $"[MockTurtle] PenUp"
        member _.PenDown() = log $"[MockTurtle] PenDown"
        }

// ======================================
// Examples
// ======================================

(*
Canvas.init()

let distance = 100.0
let angle = 120.0

// use the real canvas
let turtle = Turtle(logger,realCanvas)
turtle.Move distance
turtle.Turn angle
turtle.Move distance
turtle.Turn angle
turtle.Move distance
turtle.Turn angle
turtle.Move distance

// use mock canvas
let turtle = Turtle(logger,mockCanvas)
turtle.Move distance
turtle.Turn angle
turtle.Move distance
turtle.Turn angle

// use mock Turtle
let turtle = mockTurtle()
turtle.Move distance
turtle.Turn angle
turtle.Move distance
turtle.Turn angle

*)