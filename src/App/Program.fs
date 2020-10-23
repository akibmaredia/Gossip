open System
open Library

[<EntryPoint>]
let main argv =

    let numNodesStr = argv.[0]
    let numNodes =
        match Int32.TryParse numNodesStr with
        | true, int -> int
        | _ -> failwith "numNodes should be integer"
    let topology = argv.[1]
    let algorithm = argv.[2]

    // For testing
    // GossipSimulator.start 4 "Full" "Gossip"
    // GossipSimulator.start 4 "Line" "Gossip"
    // GossipSimulator.start 4 "2D" "Gossip"
    // GossipSimulator.start 4 "Imp2D" "Gossip"
    // GossipSimulator.start 4 "Full" "PushSum"
    // GossipSimulator.start 4 "Line" "PushSum"
    // GossipSimulator.start 4 "2DG" "PushSum"
    // GossipSimulator.start 4 "Imp2D" "PushSum"

    GossipSimulator.start numNodes topology algorithm

    printfn "finished!"

    0 // return an integer exit code
