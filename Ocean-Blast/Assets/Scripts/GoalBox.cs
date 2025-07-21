using System.Collections.Generic;
using UnityEngine;

public class GoalBox : MonoBehaviour
{
    [SerializeField] private Transform[] _openBox;
    [SerializeField] private Transform[] _closeBox;
    [SerializeField] private GameObject[] _go;
    private void Start()
    {
        SpawnGoalPrefabs();   
    }

    private void SpawnGoalPrefabs()
    {
        List<Transform> allBoxed = new List<Transform>();
        allBoxed.AddRange( _openBox );
        allBoxed.AddRange( _closeBox );

        Shuffle(allBoxed);

        int totalSpawn = _go.Length * 2;
        if(allBoxed.Count < totalSpawn)
        {
            Debug.Log("boxes not enough");
            return;
        }

        int index = 0;
        foreach(GameObject prefab in _go)
        {
            GameObject newObject = Instantiate(prefab, allBoxed[index]);
            newObject.transform.localPosition = Vector3.zero;
            index++;
        }
    }

    private void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }
    private void CloseBox()
    {
        foreach(var box in _closeBox)
        {
            SpriteRenderer sr = box.GetComponent<SpriteRenderer>();
        }
    }
}
