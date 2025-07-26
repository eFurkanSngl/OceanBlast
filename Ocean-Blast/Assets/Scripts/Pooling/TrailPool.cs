using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailPool : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailPrefab;
    [SerializeField] private int _trailCount = 10;

    private Queue<TrailRenderer> _pool = new Queue<TrailRenderer>();

    private void Awake()
    {
        for(int i = 0; i < _trailCount; i++)
        {
            CreateNewTrial();
        }
    }

    private void CreateNewTrial()
    {
        TrailRenderer trial = Instantiate(_trailPrefab, transform);
        trial.gameObject.SetActive(false);
        _pool.Enqueue(trial);
    }

    public TrailRenderer GetTrial()
    {
        if(_pool.Count == 0)
        {
            CreateNewTrial();
        }

        TrailRenderer trail = _pool.Dequeue();
        trail.Clear();
        trail.gameObject.SetActive(true);
        Debug.Log(" is worked");
        return trail;
    }

    public void ReturnTrail(TrailRenderer trail)
    {
        trail.Clear();
        trail.gameObject.SetActive(false);
        _pool.Enqueue(trail);
    }
}
