﻿// =================================
// test the command logic
// =================================

open System.IO

// start the canvas app
Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)
System.Diagnostics.Process.Start("dotnet","run") |> ignore

// setup the command writer
let commandFilePath = 
    let cwd = __SOURCE_DIRECTORY__
    let filename = "~turtlecommand.txt"
    Path.Combine(cwd,filename)

let executeCommand commands =
    File.AppendAllLines(commandFilePath,commands)

// test some commands
executeCommand ["B 100"]
executeCommand ["B 200"]
executeCommand ["S 100 100"]
executeCommand ["S 500 500"]
executeCommand ["L 10 10 30 5"]
executeCommand ["L 10 10 30 50"]
executeCommand ["L 0 0 0 100"]
executeCommand ["L 0 0 100 0"]
executeCommand ["L 0 0 0 -100"]
executeCommand ["L 0 0 -100 0"]
executeCommand ["L 10 10 15 15"; "L 20 20 25 25"; "L 30 30 35 35"; "L 40 40 45 45"; ]
executeCommand ["C"]
executeCommand ["X"]


