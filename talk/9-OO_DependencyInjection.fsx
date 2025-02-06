(* ======================================
Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/

======================================

OO-style turtle class with dependency injection

====================================== *)

#load "Common.fsx"

open System
open Common


// ======================================
// Interfaces
// ======================================

type ITurtle =
    abstract Move : Distance -> unit
    abstract Turn : Angle -> unit
    abstract PenUp : unit -> unit
    abstract PenDown : unit -> unit

type ILogger =
    abstract Info : string -> unit
    abstract Error : string -> unit

type ICanvas =
    abstract Draw : Position * Position -> unit
    abstract Clear : unit -> unit

// ======================================
// Turtle class
// ======================================

type Turtle(logger:ILogger, canvas:ICanvas) as this =

    let mutable position = initialPosition
    let mutable angle = 0.0
    let mutable penState = initialPenState

    member this.Move(distance) =
        logger.Info (sprintf "Move %0.1f" distance)
        let newPos = calcNewPosition(distance,angle,position)
        if penState = Down then
            canvas.Draw(position,newPos)

        // update the state
        position <- newPos

    member this.Turn(angleToTurn) =
        logger.Info (sprintf "Turn %0.1f" angleToTurn)
        let newAngle = calcNewAngle(angleToTurn,angle)
        // update the state
        angle <- newAngle

    member this.PenUp() =
        logger.Info "Pen up"
        penState <- Up

    member this.PenDown() =
        logger.Info "Pen down"
        penState <- Down

    // implement the interface
    interface ITurtle with
        member __.Move(distance) = this.Move(distance)
        member __.Turn(angleToTurn) = this.Turn(angleToTurn)
        member __.PenUp() = this.PenUp()
        member __.PenDown() = this.PenDown()


type TurtleFactory(logger:ILogger, canvas:ICanvas) =

    member this.NewTurtle() =
        Turtle(logger,canvas) :> ITurtle



// test

let logger =
    {new ILogger with
        member __.Info(msg) = Logger.info(msg)
        member __.Error(msg) = Logger.error(msg)
        }

let realCanvas =
    {new ICanvas with
        member __.Draw(startPos,endPos)  = Canvas.drawLine(startPos,endPos)
        member __.Clear() = Canvas.clear()
        }

let mockCanvas =
    {new ICanvas with
        member __.Draw(startPos,endPos)  = log $"[Mock] drawing from ({startPos.x},{startPos.y}) to ({endPos.x},{endPos.y})"
        member __.Clear() = () // do nothing
        }

(*
Canvas.init()

let factory = TurtleFactory(logger,realCanvas)
let turtle = factory.NewTurtle()
turtle.Move 20.0
turtle.Turn 90.0
turtle.Move 20.0
turtle.Turn 90.0
turtle.Move 20.0
turtle.Turn 90.0
turtle.Move 20.0

let factory = TurtleFactory(logger,mockCanvas)
let turtle = factory.NewTurtle()
turtle.Move 20.0
turtle.Turn 90.0
turtle.Move 20.0
*)