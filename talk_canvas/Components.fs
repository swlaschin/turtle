// Setup the window components
[<RequireQualifiedAccess>]
module Components 

open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open Avalonia.FuncUI
open Avalonia.FuncUI.Types
open Avalonia.FuncUI.DSL

let canvas () =
    Component.create ("canvas", fun ctx ->
        let canvasOutlet = ctx.useState (null, renderOnChange = false)
        let turtleClient = ctx.useState (CanvasClient.TurtleClient(canvasOutlet), renderOnChange = false)

        ctx.attrs [
            Component.dock Dock.Top
        ]

        ctx.useEffect (
            handler = (fun _ -> turtleClient.Current.Start()) ,
            triggers = [ EffectTrigger.AfterInit ]
        )

        View.createWithOutlet canvasOutlet.Set Canvas.create [
            Canvas.verticalAlignment VerticalAlignment.Stretch
            Canvas.horizontalAlignment HorizontalAlignment.Stretch
            Canvas.background Brushes.AntiqueWhite
        ] :> IView
    )

let mainView() =
    Component(fun ctx ->
        DockPanel.create [
            DockPanel.lastChildFill true
            DockPanel.children [
                canvas()
            ]
        ]
    )
