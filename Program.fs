open System
open System.Threading.Tasks
open System.IO
open Microsoft.AspNet.SignalR
open Microsoft.AspNet.SignalR.Hubs
open global.Owin
open Microsoft.Owin
open Microsoft.Owin.Hosting

[<HubName "Calculator">]
type CalcHub() =
  inherit Hub()
  member this.Calculate(id: string) =
    ()

type Startup() =

  let handleOwinContext (context: IOwinContext) =
    use writer = new StreamWriter(context.Response.Body)
    match context.Request.Method with
    | "POST" ->
      use reader = new StreamReader(context.Request.Body)
      let body = reader.ReadToEnd()
      printfn "POST:\n%s" body
      writer.Write(body)
    | _ ->
      context.Response.StatusCode <- 400
      writer.Write("Only POST supported")

  let owinHandler = fun (context: IOwinContext) (_: Func<Task>) ->
    handleOwinContext context
    Task.FromResult(null) :> Task

  member x.Configuration (app: IAppBuilder) = app.Use(owinHandler) |> ignore

[<EntryPoint>]
let main _ =
    let baseAddress = "http://localhost:8000"
    printfn "Starting calculator server on '%s'. Press <ENTER> to stop:" baseAddress
    use app = WebApp.Start<Startup>(baseAddress)
    Console.ReadLine() |> ignore
    0 // return an integer exit code
