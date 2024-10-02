using UnityEngine;

public class MoveMono : MonoBehaviour
{
    private int _amount;
    private SeekerMono[] _seekers;
    private TargetMono[] _targets;

    public void Initialize(int amount, SeekerMono[] seekers, TargetMono[] targets)
    {
        _amount = amount;

        _seekers = seekers;
        _targets = targets;
    }

    // Update is called once per frame

    void Update()
    {
        float dT = Time.deltaTime;
        for (int i = 0; i < _amount; i++)
        {
            _seekers[i].Move(dT);
            _targets[i].Move(dT);
        }
    }
}
