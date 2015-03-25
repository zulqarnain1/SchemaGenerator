// Learn more about F# at http://fsharp.net
// See the 'F# Tutorial' project for more help.
open System.IO
open System.Threading
open System
open SchemaGenerator
open System.Configuration

[<EntryPoint>]
let main argv =
    let path = ConfigurationManager.AppSettings.Get("path").ToString()

    let tb =
        { colName = "COLUMN NAME"
          colType = "DATA TYPE"
          req = "REQUIRED" }
    printfn "%A" argv
    let timer = Diagnostics.Stopwatch()
    printfn "Starting program"
    timer.Start()
    writeQueryToFile (readExcel path tb) ("./queryResult.sql")
    timer.Stop()
    printfn "Ended with duration %s mins %s secs %s ms" (timer.Elapsed.Minutes.ToString())
        (timer.Elapsed.Seconds.ToString()) (timer.Elapsed.Milliseconds.ToString())
    Console.ReadKey() |> ignore
    0 // return an integer exit code