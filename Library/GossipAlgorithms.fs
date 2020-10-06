namespace Library

open Library

module GossipAlgorithms =
    let getRandomPid lst =
        lst |> List.item (System.Random().Next(lst |> List.length))
    
    let rec gossip pid =
        let count = NetworkNode.getRumourCount pid
        if count < 10 then 
            let neighbourList = NetworkNode.getNeighbours pid "gossip"
            let randomPId = neighbourList |> getRandomPid
            NetworkNode.sendRumour randomPId
            gossip pid
    
    let pushSum pid =
        let (s, w) = NetworkNode.getSWtuple pid
        let (s, w) = (s/2, w/2)
        NetworkNode.updateSelf pid (s, w)
        let neighbourList = NetworkNode.getNeighbours pid "push-sum"
        let randomPid = neighbourList |> getRandomPid
        NetworkNode.sendSWtuple randomPid (s, w)