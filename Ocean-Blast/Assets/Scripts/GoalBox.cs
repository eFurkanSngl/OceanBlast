using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;
using Zenject;
using System.Linq;

public class GoalBox : MonoBehaviour
{
    [SerializeField] private Transform[] _openBox;
    [SerializeField] private Transform[] _closeBox;
    [SerializeField] private GameObject[] _go;

    [Header("Sound and Particle Settings")]
    [SerializeField] private Camera _mainCam;
    [SerializeField] private ParticleSystem _ps;

    [Header("Out Line Settings")]
    [SerializeField] private Vector2 _outLineSize = new Vector2(4, -4f);
    [SerializeField] private Color _outLineColor = Color.black;

    [Header("Animation Settings")]
    [SerializeField] private float _animSize = 1.2f;
    [SerializeField] private float _loopDuration = 0.7f;
    [SerializeField] private float _spawnDuration = 0.3f;
    [SerializeField] private float _delay = 0.1f;

    [Inject] private TilePool _tilePool;
    [Inject] private LauncherManager _launcherManager;
    private void OnGridSpawn()
    {
        _mainCam.GetComponent<AudioSource>().Play();
        if (_ps != null)
        {
            _ps.Play();
        }
        SpawnGoalPrefabs();
        AnimOpenBox();
    }
    private void SpawnGoalPrefabs()
    {
        // Tüm kutuları tek listede topla
        List<Transform> allBoxes = new List<Transform>();
        allBoxes.AddRange(_openBox);
        allBoxes.AddRange(_closeBox);

        List<Transform> closedBoxList = new List<Transform>(_closeBox);

        // Rastgele pozisyonlar için karıştır
        Shuffle(allBoxes);

        int totalToSpawn = _go.Length * 3;
        if (allBoxes.Count < totalToSpawn)
        {
            Debug.LogWarning("boxes not enough!");
            return;
        }

        int index = 0;
        foreach (GameObject prefab in _go)
        {
            for (int i = 0; i < 3; i++)
            {
                Transform targetBox = allBoxes[index];
                GameObject newObj = Instantiate(prefab,targetBox); // Aslında GoalItem
                newObj.transform.localPosition = Vector3.zero;

                if (closedBoxList.Contains(targetBox))
                {
                  AlphaAdjustable(newObj);
                }

                if(newObj.TryGetComponent<GoalItem>(out var goalItem)) // objede Goalıtem ulaştık
                {
                  int id = (int)goalItem.GetColor();  // rengi aldık o renge gelen ID aldık
                  int totalCount = _tilePool.TileCount[id]; // havuzda o ID ye ait toplam Count
                  goalItem.Initialize(totalCount / 3);  // Text için count / 3 ( aslında prefab sayısı )
                }

                if(newObj.TryGetComponent<GoalItemClickHandler>(out var goalItemClickHandler))
                {
                    if (_openBox.Contains(targetBox))
                    {
                        goalItemClickHandler.Set(this);
                    }
                }
                index++;
            }
        }
    }
    private void DrawOutLines(GameObject obj)
    {
        if (obj.TryGetComponent<IOutLineDrawable>(out var outLine))
        {
            outLine.DrawOutLine(_outLineColor, _outLineSize);
        }
    }
    private void RemoveOutLine(GameObject obj)
    {
        Outline outLine = obj.GetComponent<Outline>();
       if(outLine != null)
        {
            outLine.enabled = false;
        }
    }
    private void AlphaAdjustable(GameObject obj)
    {
       if(obj.TryGetComponent<IAlphaAdjustable>(out var alpha))
        {

            alpha.SetAlpha(0.5f);
        }
    }
    private void AnimOpenBox()
    {
        for (int i = 0; i < _openBox.Length; i++)
        {
            Transform box = _openBox[i];
            if (box.childCount == 0) continue;

            GameObject child = box.GetChild(0).gameObject;
            child.transform.localScale = Vector3.zero;
            child.transform.localPosition = Vector3.zero; // pozisyon sıfırdan başlasın

            float delay = i * _delay; // kutular arası delay

            // 1. Zıplayarak gel
            child.transform.DOScale(Vector3.one, _spawnDuration)
                .SetEase(Ease.OutBack)
                .SetDelay(delay)
                .OnComplete(() =>
                {
                    // 2. Sarsılma efekti
                    child.transform.DOShakePosition(0.4f, strength: new Vector3(5f, 5f, 0f), vibrato: 10, randomness: 90f, snapping: false, fadeOut: true);

                    // 3. Yoyo loop başlasın
                    child.transform.DOScale(Vector3.one * _animSize, _loopDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                });

            DrawOutLines(child);
        }
    }

        
    //Tuple shuffle 
    private void Shuffle(List<Transform> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rand = Random.Range(i, list.Count);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    public void OnGoalItemClicked(GameObject clickedObject)
    {
        if(_launcherManager.HasEmptyBox(out int emptyIndex))
        {
            OpenBoxClickEvent.ClickSoundEvent?.Invoke();
            clickedObject.transform.SetParent(_launcherManager.GetBoxTransform(emptyIndex));
            ClickedAnimation(clickedObject,emptyIndex);
            RemoveOutLine(clickedObject);
            DOTween.Kill(clickedObject);
        }
    }

    private void ClickedAnimation(GameObject gameObject, int index)
    {
        gameObject.transform.DOMove(_launcherManager.GetBoxTransform(index).position, 0.4f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                gameObject.transform.localPosition = Vector3.zero;
                gameObject.transform.localScale = Vector3.one;
                _launcherManager.PlaceGoalItem(gameObject.GetComponent<GoalItem>(), index);
            });
    }

    private void RegisterEvents() => GridManager.GridSpawnCompleted += OnGridSpawn;
    private void UnRegisterEvents() => GridManager.GridSpawnCompleted -= OnGridSpawn;

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnRegisterEvents();
    }
}
