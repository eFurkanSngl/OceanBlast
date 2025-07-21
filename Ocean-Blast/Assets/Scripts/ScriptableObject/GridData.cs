using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GridData", menuName = "OceanBlast / GridData")]
public class GridData : ScriptableObject
{
   [SerializeField] private int _width;
   [SerializeField] private int _height;
   [SerializeField] private int[] _flatLayout;
    public int[,] GetGridLayout()
    {
        int[,] grid = new int[_height, _width];

        if(_flatLayout.Length != _width * _height)
        {
            Debug.Log("Flat layout boyutu gride eş değer değil");
            return grid; 
        }

        for(int i = 0; i < _height; i++)
        {
            for(int x = 0; x < _width; x++)
            {
                grid[i,x] = _flatLayout[i * _width + x];
            }
        }
        return grid;
    }
    public void DebugTileCounts()
    {
        Dictionary<int, int> tileCounts = new();

        foreach (int id in _flatLayout)
        {
            if (id < 0) continue; // obstacle veya boş alan olabilir

            if (!tileCounts.ContainsKey(id))
                tileCounts[id] = 0;

            tileCounts[id]++;
        }

        Debug.Log("---- TILE COUNT DEBUG ----");

        foreach (var kvp in tileCounts)
        {
            Debug.Log($"Tile ID: {kvp.Key} → Count: {kvp.Value}");
        }

        Debug.Log("---------------------------");

    }
}
