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
    [SerializeField] private GameObject _bullet;
    private int _gridX;
    private int _gridY;
    private Tile[,] _tiles;
    private GridData _gridData;
    private TilePool _tilePool;
    private BulletPool _bulletPool;
    private AnimationHandler _animHandler;
    private LauncherManager _launcherManager;
    [Inject] private SignalBus _signalBus;
    private WaitForSeconds _duration = new WaitForSeconds(0.2f);
    [SerializeField] private float _animDuration = 0.2f;


    [Inject]
    public void StructInject(GridData gridData,TilePool tilePool,BulletPool bulletPool,AnimationHandler animationHandler,LauncherManager launcherManager)
    {
        _gridData = gridData;
        _tilePool = tilePool;
        _bulletPool = bulletPool;
        _animHandler = animationHandler;
        _launcherManager = launcherManager;
    }
    
    private void Start()
    {
        GenerateGrid();
        _gridData.DebugTileCounts();
        _signalBus.Fire<AnimSignalBus>();
    }

    private IEnumerator RainDownRoutine()
    {
        for (int x = _gridY - 2; x >= 0; x--) // Yukarıdan aşağıya bak
        {
            for (int y = 0; y < _gridX; y++) // Sütunları kontrol et
            {
                if (_tiles[x, y] == null) continue;

                int targetX = x;

                // Aşağı doğru boşluk kontrolü
                while (targetX + 1 < _gridY && _tiles[targetX + 1, y] == null)
                {
                    targetX++;
                }

                if (targetX != x)
                {
                    Tile fallingTile = _tiles[x, y];
                    _tiles[targetX, y] = fallingTile;
                    _tiles[x, y] = null;

                    Vector3 targetPos = GetWorldPos(targetX, y);
                    fallingTile.transform.DOMove(targetPos, 0.3f).SetEase(Ease.InOutBack);
                    yield return _duration;

                }
            }
        }
    }
    private Vector3 GetWorldPos(int row, int col)
    {
        return new Vector3(
            transform.position.x + col * _spacing,
            transform.position.y + (_gridY - 1 - row) * _spacing,  // BURADA DEĞİŞİKLİK!
            0f
        );
    }

    public IEnumerator GoalItemMatchRoutine(GoalItem goalItem)
    {
        int targetId = goalItem.GetID();

        while (goalItem != null && goalItem.CurrentCount > 0)
        {
            if (_launcherManager.IsMerging)
            {
                yield return null;
                continue;
            }

            bool found = false;

            for (int x = 0; x < _gridX; x++)
            {
                Tile tile = _tiles[_gridY - 1, x];

                if (tile == null) continue;

                int tileId = tile.GetId();

                if (tileId == targetId)
                {
                    found = true;

                    yield return StartCoroutine(PlayFireEffectRoutine(goalItem.transform.position, tile.transform.position, tile));
                    _animHandler.PlayReturnAnim(tile.gameObject, () =>
                    {
                        _tilePool.ReturnTile(tileId, tile.gameObject);
                        _signalBus.Fire<AnimSignalBus>();
                    });

                    _tiles[_gridY - 1, x] = null;

                    yield return _duration;
                    StartCoroutine(RainDownRoutine());
                    goalItem.DecreaseCount(1);
                    break;
                }
            }

            if (!found)
            {
                yield return null; // eşleşme yoksa bir frame bekle, tekrar kontrol et
            }
        }

        if (goalItem != null)
            goalItem.DestroyGoalItem(goalItem.gameObject);
    }

    private IEnumerator PlayFireEffectRoutine(Vector3 from, Vector3 to, Tile tile)
    {
        GameObject bulletObject = _bulletPool.GetBullet();
        bulletObject.transform.position = from;

        float duration = 0.3f;

        // Move Bullet
        bulletObject.transform.DOMove(to, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                _animHandler.PlayReturnAnim(tile.gameObject, () =>
                {
                    _tilePool.ReturnTile(tile.GetId(), tile.gameObject);
                });
                
                _bulletPool.ReturnBullet(bulletObject);
            });

        _signalBus.Fire<FireSoundBus>(); ;
        yield return new WaitForSeconds(duration);
    }

    private void GenerateGrid()
    {
        int[,] layout = _gridData.GetGridLayout();
        _gridY = layout.GetLength(0);
        _gridX = layout.GetLength(1);

        _tiles = new Tile[_gridY, _gridX];

        for (int x = 0; x < _gridY; x++) //Height
        {
            for (int y = 0; y < _gridX; y++) // Width
            {
                int id = layout[x, y];

                Vector3 pos = new Vector3(
                    transform.position.x + y * _spacing,
                    transform.position.y + (_gridY - 1 - x) * _spacing, 
                    0f
                );
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
                    _tiles[x,y] = tile;
                }
            }
        }
        Debug.Log("Event is worked");
        GridManagerEvents?.Invoke(_gridX, _gridY);
        GridSpawnCompleted?.Invoke();

    }
}
