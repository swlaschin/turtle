// Launch program

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Media

type MainWindow() =
    inherit HostWindow()
    do
        base.Title <- "Turtle Canvas"
        base.Content <- Components.mainView()
        base.Width <- Config.windowWidth
        base.Height <- Config.windowHeight
        base.Background <- Brushes.AntiqueWhite
        base.Topmost <- true
        base.WindowStartupLocation <- WindowStartupLocation.CenterScreen
        

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add (FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Light

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main(args: string[]) =

        let curDir = System.IO.Directory.GetCurrentDirectory()
        Util.logInfo $"[CanvasClient] Canvas is running in {curDir}" 
 
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)

    