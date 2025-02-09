namespace global

// utility functions
[<RequireQualifiedAccess>] 
module Util =

    open System

    /// Log an info message to the console
    let logInfo message = 
        printfn $"INFO: %s{message}"

    /// Log an error message to the console
    let logError message = 
        printfn $"ERROR: %s{message}"

    let parseFloat (label:string) (floatString:string)  : Validation<float,string> =
        match Double.TryParse(floatString) with
            | true, f -> Ok f
            | false, _ -> Error [$"Parse {label} with value '{floatString}' failed"]

// configuration values
[<RequireQualifiedAccess>] 
module Config =
    let margin = 10.
    let radius = 200.
    let windowWidth = 2. * (margin+radius)
    let windowHeight = 2. * (margin+radius)
    let commandFilename = "~turtlecommand.txt"
    let sleepMs = 100
