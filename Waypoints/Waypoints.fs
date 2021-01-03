namespace Waypoints

open System
open FSharpPlus
open FSharpPlus.Data
open Google.OrTools.ConstraintSolver

module WaypointsModule =

    type Waypoint = { name: string; x: int64; y: int64 }

    let manhattanDistance a b =
        Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y)

    let createMatrix (waypoints: List<Waypoint>) =
        List.map (fun x -> (List.map (manhattanDistance x)) waypoints) waypoints

    let createMatrix2 (waypoints: Waypoint []) =
        Array.map (fun x -> (Array.map (manhattanDistance x)) (waypoints)) (waypoints)

    type DataModel(distanceMatrix: int64 [] []) =
        member this.matrix = distanceMatrix
        member this.vehicleNumber = 1
        member this.depot = 0

    let sortWaypoints waypoints =
        let distanceMatrix: int64 [] [] = createMatrix2 waypoints

        let dataModel = DataModel(distanceMatrix)

        let manager: RoutingIndexManager =
            new RoutingIndexManager(dataModel.matrix.GetLength(0), dataModel.vehicleNumber, dataModel.depot)

        let routing: RoutingModel = new RoutingModel(manager)

        let takeMatrixValue (dataModel: DataModel) x y =
            dataModel.matrix.[manager.IndexToNode x].[manager.IndexToNode y]

        let transitCallbackIndex =
            (takeMatrixValue dataModel)
            |> routing.RegisterTransitCallback

        // Define cost of each arc.
        routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex)

        // Setting first solution heuristic.
        let searchParameters =
            operations_research_constraint_solver.DefaultRoutingSearchParameters()

        searchParameters.FirstSolutionStrategy <- FirstSolutionStrategy.Types.Value.PathCheapestArc

        routing.SolveWithParameters(searchParameters)
