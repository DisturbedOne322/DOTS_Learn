using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject SeekerPrefab;
    public GameObject TargetPrefab;

    public MoveMono MoveMono;
    public FindClosestMono FindClosestMono;

    public Vector2 SpawnRange;

    public int Amount = 5000;

    // Start is called before the first frame update
    void Start()
    {
        Transform[] seekerTransforms = new Transform[Amount];
        SeekerMono[] seekers = new SeekerMono[Amount];
        for (int i = 0; i < Amount; i++)
        {
            seekerTransforms[i] = Instantiate(SeekerPrefab).transform;
            seekers[i] = seekerTransforms[i].GetComponent<SeekerMono>();
            var temp = UnityEngine.Random.insideUnitCircle;
            seekers[i].Direction = new Vector3(temp.x, 0, temp.y);
            seekerTransforms[i].position = new Vector3(UnityEngine.Random.Range(-SpawnRange.x, SpawnRange.x), 0, UnityEngine.Random.Range(-SpawnRange.y, SpawnRange.y));
        }

        Transform[] targetTransforms = new Transform[Amount];
        TargetMono[] targets = new TargetMono[Amount];
        for (int i = 0; i < Amount; i++)
        {
            targetTransforms[i] = Instantiate(TargetPrefab).transform;
            targets[i] = targetTransforms[i].GetComponent<TargetMono>();
            var temp = UnityEngine.Random.insideUnitCircle;
            targets[i].Direction = new Vector3(temp.x, 0, temp.y);
            targetTransforms[i].position = new Vector3(UnityEngine.Random.Range(-SpawnRange.x, SpawnRange.x), 0, UnityEngine.Random.Range(-SpawnRange.y, SpawnRange.y));
        }

        MoveMono.Initialize(Amount, seekers, targets);
        FindClosestMono.Initialize(Amount, seekerTransforms, targetTransforms);
    }
}
