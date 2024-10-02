<H1>A small DOTS learning project</H1>

In this project, 40000 entities are spawned - 20k targets and 20k seekers.

Each frame, a seekers find the closest target to self and a Debug.DrawLine() shows the direction from one to another.

Target seeking is done as follows:
1. Sort the target positions array by the horizontal position ascending. (QuickSort algo is used)
2. Schedule an IJobParallelFor job that iterates over seekers. For each seeker:
   2.1 Using Binary search, find the closest target on the horizontal Axis.
   2.2 Find the closest target by comparing the distance on the vertical axis.
   
The project has solutions using regular MonoBehavior classes and ECS. Both solution use the same optimization techniques and Jobs, the difference is only in mono vs ecs.

The performance difference is as follow:

ECS:
![Alt text](Images/ecs.png?raw=true "Title")

MONO:
![Alt text](Images/mono.png?raw=true "Title")

Measuring the time before and after some methods calls, it appears the bottleneck for performance in mono is:
1. Copying the positions into native arrays
2. Iterating over the transforms and changing position
