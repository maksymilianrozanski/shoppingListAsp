namespace Waypoints

open System
open FSharpPlus
open FSharpPlus.Data
open Google.OrTools.ConstraintSolver

module WaypointsModule =

    type Waypoint = { name: string; x: int64; y: int64 }

    let private manhattanDistance a b =
        Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y)

    let private createMatrix (waypoints: Waypoint []) =
        Array.map (fun x -> (Array.map (manhattanDistance x)) (waypoints)) (waypoints)

    type private DataModel(distanceMatrix: int64 [] []) =
        member this.matrix = distanceMatrix
        member this.vehicleNumber = 1
        member this.depot = 0

    let private sortWaypoints waypoints =
        let distanceMatrix: int64 [] [] = createMatrix waypoints

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

        (routing.SolveWithParameters(searchParameters), routing, manager)

    let private describeSolution waypoints =
        let (assignment, routingModel, manager) = sortWaypoints waypoints

        let rec collectIndices i (indices: List<int>) =
            if (routingModel.IsEnd(i))
            then indices
            else collectIndices (assignment.Value(routingModel.NextVar(i))) (manager.IndexToNode(i) :: indices)

        (collectIndices (routingModel.Start(0)) [])
        |> List.rev

    let private waypointNamesFromIndices (waypoints: Waypoint []) resultIndices =
        List.map (fun x -> waypoints.[x].name) resultIndices

    let waypointNamesSorted (waypoints: Waypoint []) =
        waypointNamesFromIndices waypoints (describeSolution waypoints)
