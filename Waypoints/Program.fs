namespace Waypoints

open System

module WaypointsMain =

    open WaypointsModule

    let from whom = sprintf "from %s" whom

    let waypoints =
        [ { name = "point a"; x = 0L; y = 0L }
          { name = "point b"; x = 0L; y = 3L }
          { name = "point c"; x = 10L; y = 10L }
          { name = "point d"; x = 20L; y = 30L } ] |> List.toArray

    [<EntryPoint>]
    let main argv =
        let message = from "F#" // Call the function
        printfn "Hello world %s" message

        let result = describeSolution waypoints
        printf "result %A" result
        
        let waypointNames  = waypointNamesFromIndices waypoints result
        printf "waypoints names in order %A" waypointNames

        0 // return an integer exit code
