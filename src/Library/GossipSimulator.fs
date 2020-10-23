namespace Library

module GossipSimulator =

    open System
    open Akka.FSharp
    open Akka.Actor
    open Library.Messages

    // type PrintActor () =
    //     inherit Actor()

    //     override x.OnReceive(message: obj) =
    //         let sender = x.Sender

    //         match message with
    //             | :? ResultType as input ->
    //                 let actor = input.Done
    //                 printfn "%A has converged" actor
    //             | _ -> failwith "Failed to Print"


    let private populateNeighbours algorithm actorList getNeighbours =
        actorList
        |> List.iter (fun (actor: IActorRef) ->
            let neighbours = getNeighbours actor actorList
            match algorithm with
            | GossipAlgo ->
                actor <! GossipInitiateNeighbours neighbours
            | PushSumAlgo ->
                actor <! PushSumInitiateNeighBours neighbours
        )

    let start (numNodes: int) (topology: string) (algorithm: string) =
        let actorSystem = System.create "GossipSimulatorSystem" (Configuration.load())
        let inbox = Inbox.Create(actorSystem)
        let actorList =
            let getActorList func =
                match topology with
                | TopoFull | TopoLine ->
                    Topologies.generateActors actorSystem numNodes func
                | Topo2DGrid | TopoImperfect2DGrid ->
                    Topologies.generateActorsWith2DGrid actorSystem numNodes func

            match algorithm with
            | GossipAlgo -> getActorList (GossipNode.computeGossip inbox)
            | PushSumAlgo -> getActorList (GossipNode.computePushSum inbox)

        // currying
        let startPopulateNeighbours =  populateNeighbours algorithm actorList
        match topology with
            | TopoFull ->
                startPopulateNeighbours Topologies.fetchFullNeighbours
            | TopoLine ->
                startPopulateNeighbours Topologies.fetchLineNeighbours
            | Topo2DGrid ->
                startPopulateNeighbours Topologies.fetch2DGridNeighbours
            | TopoImperfect2DGrid ->
                startPopulateNeighbours Topologies.fetchImperfect2DGridNeighbours

        let pickedRandomActor =
            actorList |> List.item (Random().Next(actorList |> List.length))

        let watch = Diagnostics.Stopwatch()
        watch.Start()

        match algorithm with
        | GossipAlgo ->
            inbox.Send(pickedRandomActor, GossipRumour)
        | PushSumAlgo ->
            inbox.Send(pickedRandomActor, PushSumRumour (0.0, 0.0))

        let rec receiveResult count =
            let res = inbox.ReceiveAsync() |> Async.AwaitTask |> Async.RunSynchronously

            match res with
            | :? ResultType ->
                // let printactor = actorSystem.ActorOf(Props(typedefof<PrintActor>), "printactor")
                // let task = printactor <? res
                // Async.RunSynchronously(task)
                if count = numNodes then
                    watch.Stop()
                    printfn "Execution Time: %A ms" watch.ElapsedMilliseconds
                else
                    receiveResult (count + 1)
            | _ ->
                receiveResult count

        receiveResult 1
