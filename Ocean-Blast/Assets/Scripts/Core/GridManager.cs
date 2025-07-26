using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class GridManager : MonoBehaviour
{
    public static event UnityAction<int, int> GridManagerEvents;
    public static event UnityAction GridSpawnCompleted;
    [SerializeField] private float _spacing = 1f;
    [SerializeField] private Camera _mainCam;
    private GridData _gridData;
    private TilePool _tilePool;
    [Inject] private SignalBus _signalBus;

    [Inject]
    public void StructInject(GridData gridData,TilePool tilePool)
    {
        _gridData = gridData;
        _tilePool = tilePool;
    }
    
    private void Start()
    {
        GenerateGrid();
        _gridData.DebugTileCounts();
        _signalBus.Fire<AnimSignalBus>();
    }

    private void GenerateGrid()
    {
        int[,] layout = _gridData.GetGridLayout();
        int gridY = layout.GetLength(0);
        int gridX = layout.GetLength(1);

        for(int x = 0; x < gridY; x++) //Height
        {
            for(int y = 0; y < gridX; y++) // Width
            {
                int id = layout[x, y];

                Vector3 pos = new Vector3(transform.position.x + y * _spacing, transform.position.y + x * _spacing, 0f);
                GameObject newTile = _tilePool.GetTile(id);
                if(newTile == null)
                {
                    Debug.Log("TilePOOL did not return tile for ID");
                    continue;
                }
                newTile.transform.position = pos;
                newTile.transform.SetParent(transform);

                Tile tile = newTile.GetComponent<Tile>();
                if(tile != null)
                {
                    tile.Initialize((Tile.TileColor)id);
                }
            }
        }
        Debug.Log("Event is worked");
        GridManagerEvents?.Invoke(gridX, gridY);
        GridSpawnCompleted?.Invoke();

    }
}
