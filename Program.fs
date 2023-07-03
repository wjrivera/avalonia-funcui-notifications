// For more information see https://aka.ms/fsharp-console-apps
namespace avaloniafuncuinotifications

open Elmish
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Themes.Fluent
open Avalonia.FuncUI.Hosts
open Avalonia.Controls.Notifications
open Avalonia.FuncUI
open Avalonia.FuncUI.Elmish
open Avalonia.FuncUI.DSL

module Main =
    type State = {
        Random: Option<string>
        NotificationManager: INotificationManager
    }

    type Msg =
        | RandomMsg

    let init (topLevel: TopLevel) (): State * Cmd<Msg>  =
        let windowManager = new WindowNotificationManager(topLevel)
        let notification = new Notification("Title", "Message", NotificationType.Information)        
        windowManager.Show notification
        let state = { Random=(Some "random"); NotificationManager=windowManager}
        state, Cmd.none

    let update (msg: Msg) (state: State): State * Cmd<Msg> =
        let notification = new Notification("Title", "Message", NotificationType.Information)        
        state.NotificationManager.Show notification
        match msg with
        | RandomMsg
        | _ -> state, Cmd.none

    let view (state: State) dispatch =

        let button = Button.create [
            Button.content "Submit"
            Button.onClick (fun _ -> dispatch RandomMsg)
        ]

        Border.create [
            Border.dock Dock.Right            
            Border.margin 10.   
            Border.child button
        ]

type MainWindow() as this =
    inherit HostWindow()

    do
        base.Title <- "application"

        let topLevel = TopLevel.GetTopLevel(this)
        let windowManager = new WindowNotificationManager(topLevel)
        let notification = new Notification("Title", "Message", NotificationType.Information)
        windowManager.Show notification
                
        Elmish.Program.mkProgram (Main.init topLevel) Main.update Main.view
        |> Program.withConsoleTrace
        |> Program.withHost this
        |> Program.runWithAvaloniaSyncDispatch ()

type App() =
    inherit Application()

    override this.Initialize() =
        this.Styles.Add(FluentTheme())
        this.RequestedThemeVariant <- Styling.ThemeVariant.Dark

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime -> desktopLifetime.MainWindow <- MainWindow()
        | _ -> ()

module Program =

    [<EntryPoint>]
    let main (args: string[]) =
        AppBuilder
            .Configure<App>()
            .UsePlatformDetect()
            .UseSkia()
            .StartWithClassicDesktopLifetime(args)


