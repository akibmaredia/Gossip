# COP-5615 - Distributed Operating Systems

# Gossip Simulator

## Instructions for running the code

* Go to the src/App directory or use ```$ cd src/App``` command on the terminal
* type ``` dotnet run --project App.fsproj -- <number of nodes> "<topology>" "<algorithm>" ```
* Here <`number of nodes`> can be any integer,
* <`topology`> is a string and one of the four topologies
  * "full"
  * "line"
  * "2d"
  * "imp2d"

* <`algorithm`> is a string and one of the two algorithms
  * "gossip"
  * "push-sum"

* example command  ```dotnet run --project App.fsproj -- 10 "full" "gossip"``` or ```dotnet run --project App.fsproj -- 10 "full" "push-sum"```

## Whats working

All nodes are converging every time for Gossip in all topologies. The number of nodes that can be handled depends on the system as there is a limit on the number of processes.

For both push-sum and gossip algorithms, the time taken by topologies is in the following order
full < imp2d < 2d < line.

There are instances where imp2D and full have same time but that depends on the number of processes currently running. In ideal conditions where the CPU is not involved in large number of tasks, the full topology will take less time than imp2D.

Line topology is very time consuming for both pushsum and gossip algorithms. It takes a large amount of time for nodes in line topology to converge

Gossip algorithm works fine for upto 10000 nodes for Full, 2D and Imp2D topologies.

On the other hand, in push-sum, the nodes take relatively more time since the sum has to travel from one end of the network to another end which is not possible when the network is too large.

## Largest Network

The largest network that we could manage was 10K nodes for Full, 2D and Imp2D

## Program flow

The entry point is Program.fs which takes the command line arguments and sends these arguments to the GossipSimulator.

The GossipSimulator then creates an actor system and according to the topology provided spawns the actors. The Full and line topologies spawn actors in the similar naming convention and 2d and imp2d spawn actors in a similar naming convention. Now the difference comes when providing neighbours to the spawned actors.

For full topology, each actor is provided by the list of references to all the other actors. WhereSas for line each actor is provided by the left and right node actor except for the first and the last actor.

For 2D and imp2d topology, each actor has a row number and a column number. The difference between these topologies is such that imp2d has 1 extra neighbour per node. And this extra neighbor is randomized.

The actors are defined in the GossipNode.fs file. The actor structure is different for pushsum and gossip algorithms.

The GossipSimulator start function keeps a track of the nodes that have converged and as soon as the number of converged nodes becomes equal to the number of total nodes, we stop the timer and print out the time.
