(*
Common.fsx

Part of "Thirteen ways of looking at a turtle"
Talk and video: http://fsharpforfunandprofit.com/turtle/

*)

open System


// ======================================
// Constants
// ======================================
[<AutoOpen>]
module Constants =
    // the distance from the origin to the edge of the canvas
    // So canvas width = 2 * radius    
    let canvasRadius = 200.0

// ======================================
// Logger
// ======================================

/// A crude logger to log to the console
module Logger =

    let info msg = printfn $"INFO %s{msg}"
    let warn msg = printfn $"WARN %s{msg}"
    let error msg = printfn $"ERROR %s{msg}"
    
    let debugActive = false // toggle this to see debug messages
    let debug msg =
        if debugActive then
            printfn $"DEBUG %s{msg}"

// ======================================
// Common types used throughout the scripts
// ======================================
[<AutoOpen>]
module DomainTypes =
    /// An alias for a float
    type Distance = float

    /// An alias for a float of Degrees
    type Angle  = float

    /// Enumeration of available pen states
    type PenState = Up | Down

    /// A structure to store the (x,y) coordinates
    /// This is NOT the same as canvas coords -- it has (0,0) at the middle/origin
    type TurtlePosition = {x:float; y:float}
        with override this.ToString() = $"(%g{this.x},%g{this.y})"

    // format Position nicely 
    fsi.AddPrinter(fun (x : TurtlePosition) -> x.ToString())

    /// Represent a change in a turtle position
    type TurtleDelta = {dx:float; dy:float}
        with override this.ToString() = $"(%g{this.dx},%g{this.dy})"
    
    /// returned from calculations that could be bounded
    type BoundedResult = {
        distanceMoved : Distance
        wasBounded : bool
    }

// ======================================
// Helper functions for turtle calculations
// ======================================

[<AutoOpen>]
module TurtleCalculations =

    /// round a float to two places to make it easier to read
    let round2 (flt:float) = Math.Round(flt,2)

    /// Calculate a delta from a distance and angle
    let calcDelta(distance:Distance,angle:Angle) :TurtleDelta=
        // Convert degrees to radians with 180.0 degrees = 1 pi radian
        let angleInRads = angle * (Math.PI/180.0) 
        // return a delta
        {dx=(distance * cos angleInRads); dy=(distance * sin angleInRads)}

    /// Calculate a new position using the delta
    let applyDelta (delta:TurtleDelta) (pos:TurtlePosition) :TurtlePosition=
        {x=round2(pos.x + delta.dx); y=round2(pos.y + delta.dy)}

    /// Scale a delta
    let scaleDelta scale (delta:TurtleDelta) :TurtleDelta =
        {dx=round2(scale * delta.dx); dy=round2(scale * delta.dy)}
    
    /// Calculate a new (unbounded) position from the current position given an angle and a distance
    /// calcNewPosition :   Distance * Angle * Position -> Position
    let calcNewPosition(distance:Distance,angle:Angle,currentPos:TurtlePosition) =
        let vector = calcDelta(distance,angle)
        currentPos |> applyDelta vector

    /// keep a dimension within bounds, return true if it was bounded
    let boundedAxis max min x =
        if x > max then
            max, true
        else if x < min then
            min, true
        else
            x, false

    /// keep a delta within a bounding box by scaling it down if needed, return true if it was bounded
    let boundedDelta topLeftBound bottomRightBound delta =
        let dxBounded, dxWasBounded = delta.dx |> boundedAxis topLeftBound.x bottomRightBound.x 
        let dyBounded, dyWasBounded = delta.dy |> boundedAxis topLeftBound.y bottomRightBound.y
        
        // calculate the scale factors if any
        let xScale = if dxWasBounded then dxBounded / delta.dx else 1.0
        let yScale = if dyWasBounded then dyBounded / delta.dy else 1.0
        
        // choose the smaller scale factor
        let scale = if xScale < yScale then xScale else yScale
        
        // scale both dx and dy by the scale factor
        let scaledDelta = delta |> scaleDelta scale

        // Logger.debug $"[bounded X] pos.x={delta.dx} xBounded={dxBounded} xScale={xScale}"
        // Logger.debug $"[bounded Y] pos.y={delta.dy} yBounded={dyBounded} yScale={yScale}"
        // Logger.debug $"[bounded Result] new dx={scaledDelta.dx} new dy={scaledDelta.dy} scale={scale}"
        
        scaledDelta, (dxWasBounded || dyWasBounded)

    /// direct distance between two points
    let pythagorianDistance pos1 pos2 =
        let sq x = x * x
        sq (pos1.x - pos2.x) + sq (pos1.y - pos2.y)
        |> Math.Abs
        |> Math.Sqrt
        |> round2

    /// Calculate a new position from the current position but keep it bounded
    /// Also return the actual distance moved and whether it was bounded
    let calcNewPositionBounded(distance,angle,currentPos) : TurtlePosition * BoundedResult =
        // calculate a bounding box by treating the current position as delta to the canvas bounding box
        let delta = {dx = -currentPos.x; dy = -currentPos.y}
        let topLeftBound = {x = canvasRadius; y = canvasRadius} |> applyDelta delta
        let bottomRightBound = {x = -canvasRadius; y = -canvasRadius} |> applyDelta delta
        
        // calculate the bounded delta
        let delta,wasBounded =
            calcDelta(distance,angle)
            |> boundedDelta topLeftBound bottomRightBound

        let newPos = currentPos |> applyDelta delta
        let distanceMoved = pythagorianDistance currentPos newPos 
        newPos, {distanceMoved=distanceMoved; wasBounded=wasBounded}

    /// Calculate a new angle, given an angle to turn
    let calcNewAngle(angleToTurn,currentAngle) =
        (currentAngle + angleToTurn) % 360.0

    /// Default initial state of a turtle
    let initialPosition,initialPenState =
        {x=0.0; y=0.0}, Down


(*
test the bounded logic

calcNewPositionBounded(141.42, 45.0, {x=0. ; y=0.})  // dx=100 scale=1
calcNewPositionBounded(200.0, 45.0, {x=0. ; y=0.})  // dx=141.42 scale=1
calcNewPositionBounded(300.0, 45.0, {x=50. ; y=50.})  // dx=150  scale=0.707
calcNewPositionBounded(300.0, 90.0, {x=50. ; y=50.})  // dx=0,dy=150  scale=0.5
calcNewPositionBounded(300.0, 90.0, {x=50. ; y=199.})  // dx=0,dy=1  scale=0.0033
calcNewPositionBounded(300.0, 45.0, {x=50. ; y=199.})  // dx=1,dy=1  scale=0.0047
*)

// ======================================
// Canvas
// ======================================

module Canvas =
    open DomainTypes
    open System.IO

   
    // track the open canvas window
    let mutable canvasProcess : System.Diagnostics.Process = null 

    /// Build the canvas executable if needed 
    let build() =
        Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)
        System.Diagnostics.Process.Start("dotnet",@"build ../talk_canvas/canvas.fsproj")
        |> ignore
    
    // test with
    // Canvas.build()

    /// Start the canvas app if it is not already running.
    /// NOTE: Run build() first time before using this
    let init() =
        Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)
        if (isNull canvasProcess) || (canvasProcess.HasExited) then
            Logger.info "Launching canvas"
            canvasProcess <- System.Diagnostics.Process.Start("dotnet",@"../talk_canvas/bin/Debug/net8.0/canvas.dll") 
    
    // test with
    // Canvas.init()

    

    /// Define the path to the command file
    let commandFilePath =
        let cwd = __SOURCE_DIRECTORY__
        let filename = "~turtlecommand.txt"
        Path.Combine(cwd,filename)

    /// Setup the command writer
    let executeCommand commands =
        File.AppendAllLines(commandFilePath,commands)
            
    /// Clear the canvas
    let clear() =
        executeCommand ["C"]

    /// Draw a boundary on the canvas            
    let drawBoundary() =
        executeCommand [$"B {canvasRadius}"]

    /// Turn verbose logging on/off in the canvas.
    /// If turned on, each action will be logged to console.
    let verbose(v) =
        if v then
            executeCommand ["V true"]
        else            
            executeCommand ["V false"]
    // Canvas.verbose(true)
    // Canvas.verbose(false)
    
    /// Draw a line on the canvas
    let drawLine(pos1:TurtlePosition,pos2:TurtlePosition) =
        executeCommand [$"L {pos1.x} {pos1.y} {pos2.x} {pos2.y}"]

    (*
    // test some canvas actions
    Canvas.init()
    Canvas.drawBoundary()
    Canvas.drawLine( {x=  0.0; y=  0.0}, {x= 10.0; y=20.} )
    Canvas.drawLine( {x= 10.0; y= 20.0}, {x= 50.0; y=50.} )
    Canvas.drawLine( {x= 50.0; y= 50.0}, {x= -50.0; y=50.0} )
    Canvas.drawLine( {x= -50.0; y=50.0}, {x=  0.0; y=  0.0} )
    Canvas.clear()
    *)



