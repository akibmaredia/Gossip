module Algorithms

open Library.Messages


let criterion = 10.0 ** -10.0
let limitation = 10
let consecutionLimitation = 3

let gossip count =
    if count = limitation then GossipExit
    elif count < limitation then GossipContinue
    else GossipIgnore

let pushSum (oldValue: float * float) (newValue: float * float) consecution (isDone: bool)=
    let (os, ow) = oldValue
    let (ns, nw) = newValue
    let diff = abs (os/ow - ns/nw)

    if isDone then PushSumIgnore
    elif consecution >= consecutionLimitation then PushSumExit
    else
        if diff <= criterion then PushSumContinue (consecution + 1)
        else PushSumContinue 0