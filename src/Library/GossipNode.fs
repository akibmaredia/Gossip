module GossipNode

open System
open Akka.FSharp
open Akka.Actor
open Library.Messages

let private getRandomActor (lst: List<IActorRef>) =
    lst |> List.item (Random().Next(lst |> List.length))

let computeGossip (inbox: Inbox) (mailbox: Actor<GossipMessage>) =
    let rec loop (count: int) (neighbours: List<IActorRef>) =
        actor {
            // let asyncprint =
            //     async {
            //         printfn "Actor %A converged" mailbox.Self.Path
            //     }
            let! msg = mailbox.Receive()
            match msg with
            | GossipInitiateNeighbours newNeighbours ->
                return! loop count newNeighbours
            | GossipRumour ->
                match Algorithms.gossip count with
                | GossipContinue ->
                    inbox.Send(getRandomActor neighbours, GossipRumour)
                    return! loop (count + 1) neighbours
                | GossipExit ->
                    // Async.RunSynchronously(asyncprint)
                    mailbox.Sender() <! {Done = mailbox.Self.Path};
                    inbox.Send(getRandomActor neighbours, GossipRumour)
                    return! loop (count + 1) neighbours
                | GossipIgnore ->
                    inbox.Send(getRandomActor neighbours, GossipRumour)
                    return! loop count neighbours
        }
    loop 0 []


let computePushSum (inbox: Inbox) (mailbox: Actor<PushSumMessage>) =
    let actorNumber = mailbox.Self.Path.Name.Split '-' |> Array.last |> float
    let initRumour = (actorNumber, 1.0)

    let rec loop (oldRumour: float * float) (neighbours: List<IActorRef>) (consecution: int) (isDone: bool) =
        actor {
            let! msg = mailbox.Receive()
            match msg with
            | PushSumInitiateNeighBours newNeighbours ->
                return! loop oldRumour newNeighbours consecution isDone
            | PushSumRumour (ns, nw) ->
                let halfNewS = ((fst oldRumour) + ns) / 2.0;
                let halfNewW = ((snd oldRumour) + nw) / 2.0;
                let newRumour = (halfNewS, halfNewW)
                match Algorithms.pushSum oldRumour newRumour consecution isDone with
                | PushSumContinue newConsecution ->
                    inbox.Send(getRandomActor neighbours, PushSumRumour (halfNewS, halfNewW))
                    return! loop newRumour neighbours newConsecution isDone
                | PushSumExit ->
                    mailbox.Sender() <! {Done = mailbox.Self.Path};
                    inbox.Send(getRandomActor neighbours, PushSumRumour (halfNewS, halfNewW))
                    return! loop newRumour neighbours consecution true
                | PushSumIgnore ->
                    inbox.Send(getRandomActor neighbours, PushSumRumour (halfNewS, halfNewW))
                    return! loop newRumour neighbours consecution isDone
        }

    loop initRumour [] 0 false
