module WaypointsTests

open NUnit.Framework
open Waypoints
open WaypointsModule

[<SetUp>]
let Setup () = ()

[<Test>]
let Test1 () = Assert.Pass()

[<Test>]
let ``should return waypoint names sorted in expected order`` () =
    let waypoints =
        [| { name = "a"; x = 0L; y = 0L }
           { name = "c"; x = 12L; y = 13L }
           { name = "b"; x = 2L; y = 3L }
           { name = "d"; x = 122L; y = 133L }
           { name = "checkout"
             x = 200L
             y = 200L } |]

    let expectedOrder = [ "a"; "b"; "c"; "d" ]

    let result = waypointNamesSorted waypoints

    Assert.AreEqual(expectedOrder, result)

[<Test>]
let ``should start at index-0 waypoint`` () =
    let waypoints =
        [| { name = "a"; x = 99L; y = 99L }
           { name = "c"; x = 12L; y = 13L }
           { name = "b"; x = 2L; y = 3L }
           { name = "d"; x = 122L; y = 133L }
           { name = "checkout"
             x = 200L
             y = 200L } |]

    let result = waypointNamesSorted waypoints

    Assert.AreEqual("a", result.Head)

[<Test>]
let ``should return waypoint name sorted in expected order when starting point is repeated`` () =
    let waypoints =
        [| { name = "a"; x = 1L; y = 1L }
           { name = "b"; x = 12L; y = 13L }
           { name = "same as a"; x = 1L; y = 1L }
           { name = "c"; x = 200L; y = 200L }
           { name = "d"; x = 333L; y = 333L }
           { name = "checkout"
             x = 334L
             y = 334L } |]

    let result = waypointNamesSorted waypoints

    Assert.AreEqual("a", result.Head, "should always start at index-0 waypoint")
    Assert.AreEqual("same as a", result.Item(1))
    Assert.AreEqual("b", result.Item(2))
    Assert.AreEqual("c", result.Item(3))
    Assert.AreEqual("d", result.Item(4))

[<Test>]
let ``should return waypoints sorted in expected order when contains repeated points`` () =

    let waypoints =
        [| { name = "a"; x = 1L; y = 1L }
           { name = "b"; x = 12L; y = 13L }
           { name = "same as b"; x = 12L; y = 13L }
           { name = "c"; x = 200L; y = 200L }
           { name = "d"; x = 333L; y = 333L }
           { name = "checkout"
             x = 334L
             y = 334L } |]

    let result = waypointNamesSorted waypoints

    Assert.AreEqual("a", result.Head, "should always start at index-0 waypoint")
    let resultIndexOne = result.Item(1)
    let resultIndexTwo = result.Item(2)
    Assert.AreNotEqual(resultIndexOne, resultIndexTwo)
    //waypoints with equal coordinates can be sorted in any order
    Assert.True
        (resultIndexOne = "b"
         || resultIndexOne = "same as b")

    Assert.True
        (resultIndexTwo = "b"
         || resultIndexTwo = "same as b")

    Assert.AreEqual("c", result.Item(3))
    Assert.AreEqual("d", result.Item(4))

[<Test>]
let ``should set last waypoint near checkout (checkout near 'c' waypoint)`` () =
    let waypoints =
        [| { name = "a"; x = 50L; y = 0L }
           { name = "b"; x = 10L; y = 1L }
           { name = "c"; x = 110L; y = 1L }
           { name = "checkout"; x = 111L; y = 1L } |]

    let result = waypointNamesSorted waypoints

    let expectedOrder = [ "a"; "b"; "c" ]
    Assert.AreEqual(expectedOrder, result)

[<Test>]
let ``should set last waypoint near checkout2 (checkout near 'b' waypoint)`` () =
    let waypoints =
        [| { name = "a"; x = 50L; y = 0L }
           { name = "b"; x = 10L; y = 1L }
           { name = "c"; x = 110L; y = 1L }
           { name = "checkout"; x = 11L; y = 1L } |]

    let result = waypointNamesSorted waypoints

    let expectedOrder = [ "a"; "c"; "b" ]
    Assert.AreEqual(expectedOrder, result)
