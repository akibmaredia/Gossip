namespace Library.Messages

open Akka.Actor
open Akka.FSharp

[<AutoOpen>]
module Messages =

    type GossipMessage =
    | GossipRumour
    | GossipInitiateNeighbours of List<IActorRef>

    type PushSumMessage =
    | PushSumRumour of float * float
    | PushSumInitiateNeighBours of List<IActorRef>

    type AlgorithmGossipStatus =
    | GossipContinue
    | GossipExit
    | GossipIgnore

    type AlgorithmPushSumStatus =
    | PushSumContinue of int
    | PushSumExit
    | PushSumIgnore

    type AlgorithmType =
    | GossipAlgo
    | PushSumAlgo

    type ResultType = {
        Done: ActorPath
    }

    let (|GossipAlgo|PushSumAlgo|) (name: string) =
        if name.ToUpper() = "Gossip".ToUpper() then GossipAlgo
        elif name.ToUpper() = "Push-Sum".ToUpper() then PushSumAlgo
        else failwith "Unknown type of algorithm"

    type TopologyTypes =
    | TopoFull
    | TopoLine
    | Topo2DGrid
    | TopoImperfect2DGrid

    let (|TopoFull|TopoLine|Topo2DGrid|TopoImperfect2DGrid|) (name: string) =
        if name.ToUpper() = "Full".ToUpper() then TopoFull
        elif name.ToUpper() = "Line".ToUpper() then TopoLine
        elif name.ToUpper() = "2d".ToUpper() then Topo2DGrid
        elif name.ToUpper() = "imp2d".ToUpper() then TopoImperfect2DGrid
        else failwith "Unknown type of topology"
