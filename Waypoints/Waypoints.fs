namespace Waypoints

open System
open FSharpPlus
open FSharpPlus.Data

module WaypointsModule =

    type Waypoint = { name: string; x: int; y: int }

    let manhattanDistance a b =
        Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y)

    let createMatrix (waypoints: List<Waypoint>) =
        List.map (fun x -> (List.map (manhattanDistance x)) waypoints) waypoints
