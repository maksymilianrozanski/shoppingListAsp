namespace Waypoints

open System

module WaypointsMain =

    open WaypointsModule

    let from whom = sprintf "from %s" whom

    let (waypoints: List<Waypoint>) =
        [ { name = "point a"; x = 0; y = 0 }
          { name = "point b"; x = 0; y = 3 }
          { name = "point c"; x = 10; y = 10 }
          { name = "point d"; x = 20; y = 30 } ]

    [<EntryPoint>]
    let main argv =
        let message = from "F#" // Call the function
        printfn "Hello world %s" message

        let matrix = createMatrix waypoints

        0 // return an integer exit code
