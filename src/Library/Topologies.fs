module Topologies

open System
open Akka.FSharp
open Akka.Actor

let private actorBasicName = "my-actor-"

let generateActors (system: ActorSystem) numNodes func =
    [1 .. numNodes]
    |> List.map (fun i ->
        let actorNumber = actorBasicName + (i |> string)
        spawn system actorNumber func
    )

let generateActorsWith2DGrid (system: ActorSystem) numNodes func =
    let numRows = ceil (sqrt (numNodes |> float)) |> int
    [1 .. numRows]
    |> List.collect (fun rowPos ->
        [ for columnPos in 1 .. numRows ->
            let actorNumber = actorBasicName + ((rowPos * numRows) + columnPos |> string)
            spawn system actorNumber func ]
    )

let fetchFullNeighbours (pickedActor: IActorRef) (actorList: List<IActorRef>) =
    actorList
    |> List.filter (fun a -> a <> pickedActor)

let fetchLineNeighbours (pickedActor: IActorRef) (actorList: List<IActorRef>) =
    let lastIndex = (actorList |> List.length) - 1
    let currentIndex = actorList |> List.findIndex (fun a -> a = pickedActor)
    if pickedActor = actorList.Head then [ actorList.[1] ]
    elif pickedActor = actorList.[lastIndex] then [ actorList.[lastIndex - 1] ]
    else [ actorList.[currentIndex - 1]; actorList.[currentIndex + 1] ]

let fetch2DGridNeighbours (pickedActor: IActorRef) (actorList: List<IActorRef>) =
    let rowSize = actorList.Length |> float |> sqrt |> int
    let lastRowPos = rowSize - 1
    let lastColPos = rowSize - 1
    let currentIndex = actorList |> List.findIndex (fun x -> x = pickedActor)
    let row = currentIndex / rowSize
    let col = currentIndex % rowSize

    let getIndexByCoord (row, col) = row * rowSize + col

    let getTwoIndexes firstCoord secondCoord =
        getIndexByCoord secondCoord :: [getIndexByCoord firstCoord]

    let getThreeIndexes firstCoord secondCoord thirdCoord =
        getIndexByCoord thirdCoord :: getTwoIndexes firstCoord secondCoord

    let getFourIndexes firstCoord secondCoord thirdCoord fourthCoord =
        getIndexByCoord fourthCoord :: getThreeIndexes firstCoord secondCoord thirdCoord

    let neighboursPos =
        match (row, col) with
        | (0, 0) ->
            getTwoIndexes (1, 0) (0, 1)
        | (0, colPos) when colPos = lastColPos ->
            getTwoIndexes (1, colPos) (0, colPos - 1)
        | (rowPos, 0) when rowPos = lastRowPos ->
            getTwoIndexes (rowPos - 1, 0) (rowPos, 1)
        | (rowPos, colPos) when rowPos = lastRowPos && colPos = lastColPos ->
            getTwoIndexes (rowPos - 1, colPos) (rowPos, colPos - 1)
        | (0, colPos) ->
            getThreeIndexes (0, colPos - 1) (1, colPos) (0, colPos + 1)
        | (rowPos, 0) ->
            getThreeIndexes (rowPos - 1, 0) (rowPos, 1) (rowPos + 1, 0)
        | (rowPos, colPos) when rowPos = lastRowPos ->
            getThreeIndexes (rowPos, colPos - 1) (rowPos - 1, colPos) (rowPos, colPos + 1)
        | (rowPos, colPos) when colPos = lastColPos ->
            getThreeIndexes (rowPos - 1, colPos) (rowPos, colPos - 1) (rowPos + 1, colPos)
        | (rowPos, colPos) ->
            getFourIndexes (rowPos - 1, colPos) (rowPos, colPos + 1) (rowPos + 1, colPos) (rowPos, colPos - 1)

    neighboursPos |> List.map (fun pos -> actorList.[pos])

let fetchImperfect2DGridNeighbours (pickedActor: IActorRef) (actorList: List<IActorRef>) =
    let neighbourList = fetch2DGridNeighbours pickedActor actorList
    let actorListWithNeighbours = actorList |> List.except neighbourList
    let randomActor = actorListWithNeighbours |> List.item (Random().Next(actorListWithNeighbours |> List.length))
    randomActor :: neighbourList
