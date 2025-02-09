[<RequireQualifiedAccess>] 
module CanvasClient

open System
open System.IO

open Avalonia.Controls
open Avalonia
open Avalonia.Media
open Avalonia.Controls.Shapes
open Avalonia.FuncUI
open Avalonia.VisualTree
open Microsoft.VisualBasic

type ExecuteCommandResult = Stop | Continue

type TurtleClient(canvas:IWritable<Canvas>) =

    // if true, log all the actions
    let mutable verbose = true
    let logInfo str = if verbose then Util.logInfo str 

    let commandFilePath = 
        let cwd = Directory.GetCurrentDirectory()
        Path.Combine(cwd,Config.commandFilename)
   
    let strokeThickness = 2.0

    /// Convert from a turtle position to screen position.
    /// In turtle coordinates where (0,0) is origin and y increases north.
    /// In screen position where (0,0) is top left corner, and y increases south.
    let convertFromTurtlePosition (canvasSize:Size) (turtlePosition:Point) =
        let x = turtlePosition.X + (canvasSize.Width/2.0)
        let y = -turtlePosition.Y + (canvasSize.Height/2.0)
        Point(x,y)
        
        
    /// primitive - draw a line on canvas using the given brush
    let primDrawFromTo (canvas:IWritable<Canvas>) brush startPoint endPoint =
        Avalonia.Threading.Dispatcher.UIThread.Post(fun () ->
            if canvas.Current <> null then
                let canvasSize = canvas.Current.Bounds.Size
                let line = Line(
                    StartPoint = convertFromTurtlePosition canvasSize startPoint,
                    EndPoint = convertFromTurtlePosition canvasSize endPoint,
                    Stroke = brush,
                    StrokeThickness = strokeThickness,
                    StrokeLineCap = PenLineCap.Round
                    )
                canvas.Current.Children.Add(line)
        )

    // primitive - clear the canvas
    let primClearCanvas (canvas:IWritable<Canvas>) =
        logInfo $"[CanvasClient] clear canvas"
        Avalonia.Threading.Dispatcher.UIThread.Post(fun () ->
            if canvas.Current <> null then
                canvas.Current.Children.Clear()
        )

    // resize window
    let primResize (canvas:IWritable<Canvas>) width height =
        Avalonia.Threading.Dispatcher.UIThread.Post(fun () ->
            if canvas.Current <> null then
                let main = canvas.Current.GetVisualRoot() :?> Window
                main.Width <- width
                main.Height <- height
        )
        logInfo $"[CanvasClient] resize {width},{height}"

        
    // draw a turtle line
    let drawLine canvas startPoint endPoint =
        let brush = Brushes.Black
        primDrawFromTo canvas brush startPoint endPoint
        logInfo $"[CanvasClient] turtle line from {startPoint} to {endPoint}"

    // draw border on canvas with given radius
    let drawBorder canvas radius =
        let brush = Brushes.IndianRed
        let topLeft = Point(-radius,radius)
        let topRight = Point(radius,radius)
        let bottomLeft = Point(-radius,-radius)
        let bottomRight = Point(radius,-radius)
        primDrawFromTo canvas brush topLeft topRight
        primDrawFromTo canvas brush topRight bottomRight
        primDrawFromTo canvas brush bottomRight bottomLeft
        primDrawFromTo canvas brush bottomLeft topLeft
        logInfo $"[CanvasClient] draw border {radius}"

    let executeCommand (command:string) =
        let parts = command.Split([|' '|],StringSplitOptions.TrimEntries) |> List.ofArray
        match parts with
        
        // stop processing
        | ["X"] ->
            Stop
            
        // empty command            
        | [""] ->            
            Continue // ignore no command
            
        // clear canvas            
        | ["C"] ->
            primClearCanvas canvas
            Continue
            
        // draw border            
        | ["B"; radiusStr] ->
            let radiusOrError = validation {
                let! radius = Util.parseFloat "radius" radiusStr
                return radius
            }
            match radiusOrError with
            | Ok radius ->
                drawBorder canvas radius
            | Error msgs ->
                for msg in msgs do
                    Util.logError msg
            Continue
            
        // draw line            
        | ["L"; x1; y1; x2; y2] ->
            let parsedPoints = validation {
                let! x1 = Util.parseFloat "x1" x1
                and! y1 = Util.parseFloat "y1" y1
                and! x2 = Util.parseFloat "x2" x2
                and! y2 = Util.parseFloat "y2" y2
                return (Point(x1,y1), Point(x2,y2))
            }
            match parsedPoints with
            | Ok (startP, endP) ->
                drawLine canvas startP endP
            | Error msgs ->
                for msg in msgs do
                    Util.logError msg
            Continue
            
        // resize            
        | ["S"; width; height] ->
            let parsedSize = validation {
                let! width = Util.parseFloat "width" width
                and! height = Util.parseFloat "height" height
                return (width,height)
            }
            match parsedSize with
            | Ok (width,height) ->
                primResize canvas width height
            | Error msgs ->
                for msg in msgs do
                    Util.logError msg
            Continue

        // set verbose            
        | ["V"; arg] ->            
            verbose <- (arg = "true") // anything else => not verbose
            Util.logInfo $"[CanvasClient] set verbose: {verbose}"
            Continue

        // unknown            
        | _ ->            
            Util.logError $"[CanvasClient] Command unknown: {command}"
            Continue

    // read commands from the command file
    let readCommandLoop() =
        logInfo $"[CanvasClient] Reading commands from '{commandFilePath}'"

        let rec loop action =
            match action with
            | Stop ->
                logInfo $"[CanvasClient] Exiting"
            | Continue -> 
                System.Threading.Thread.Sleep(Config.sleepMs)
                try
                    // existing file
                    let commands = File.ReadAllLines(commandFilePath)
                    //logInfo $"[CanvasClient] reading %A{commands}"
                    File.Delete(commandFilePath)
                    let action =
                        commands
                        |> Array.map executeCommand
                        |> Array.head
                    loop action
                with
                | :? FileNotFoundException ->
                    //logInfo $"[CanvasClient] nothing to do"
                    () // ignore

                loop Continue
                
        // start loop
        loop Continue

    // start the background command reading loop
    member this.Start() = 
        System.Threading.Tasks.Task.Factory.StartNew(readCommandLoop)
        |> ignore        // don't care about result

