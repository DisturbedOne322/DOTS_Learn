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
<H2>ECS</H2>
![ecs](https://github.com/user-attachments/assets/61c88369-7480-44b1-bc29-a9234dfaeaf8)

<H2>MONO</H2>
![mono](https://github.com/user-attachments/assets/32068107-9a8a-4ac3-ae23-b14e9582ec75)
