## Canvas for interactive talk

This [Avalonia app](https://avaloniaui.net/) is the canvas used by the interactive talk.

When commands are executed interactively from the scripts in the `\talk` directory, 
the commands are sent to this canvas to be displayed on screen.

## How the scripts communicate with the canvas

The IPC mechanism used is crude but effective:
* The talk script writes some "command" lines to a file
* The canvas listens for changes to that file and, on a change
* reads the file and performs the action for each command.
* then deletes the file

The file is in the `.\talk\` directory and is called `~turtlecommand.txt`

The commands are

* `C` - clear the canvas
* `B radius` - draw a square border at given radius. Example: `B 200` draws a border with radius 200.
* `L x1 y1 x2 y2` - draw a line from (x1,y1) to (x2,y2). Example: `L 0 10 30 40` draws a line from (0,10) to (30,40)
   where x1, y1 etc. are floats
* `V true`, `V false` - toggle verbose logging 
* `X` - exit the command loop


NOTE: all points are specified in turtle coordinates where (0,0) is origin and y increases north.
For display, they are converted to a screen position where (0,0) is top left corner, and y increases south.


## How to start the canvas app

First build the app:

```
cd ..\talk_canvas
dotnet build
```

then run, using the  `.\talk` directory as the current directory

```
cd ..\talk
dotnet ..\talk_canvas\bin\Debug\net8.0\canvas.dll
```

Both these operations can also be done from `Canvas` module 
in the `Common.fsx` script file in the `talk` directory.

See `Canvas.build()` and `Canvas.init()`

## Standalone testing

There is a [`test.fsx`](test.fsx) script for doing some interactive testing.