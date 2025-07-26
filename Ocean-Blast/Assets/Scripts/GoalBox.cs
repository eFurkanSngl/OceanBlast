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
    private AudioSource _audioSource;

    [Header("Out Line Settings")]
    [SerializeField] private Vector2 _outLineSize = new Vector2(4, -4f);
    [SerializeField] private Color _outLineColor = Color.black;

    [Header("Animation Settings")]
    [SerializeField] private float _animSize = 1.2f;
    [SerializeField] private float _loopDuration = 0.4f;
    [SerializeField] private float _spawnDuration = 0.3f;
    [SerializeField] private float _delay = 0.1f;

    [Inject] private TilePool _tilePool;
    [Inject] private LauncherManager _launcherManager;
    [Inject] private SignalBus _signalBus;
    [Inject] private TrailPool _trailPool;

    private List<Transform> closedBoxList = new List<Transform>();
    private List<Transform> allBoxes = new List<Transform>();
    private void Awake()
    {
        _audioSource = _mainCam.GetComponent<AudioSource>();
        allBoxes.AddRange(_openBox);
        allBoxes.AddRange(_closeBox);

        closedBoxList.AddRange(_closeBox);
    }
    private void OnGridSpawn()
    {
        _audioSource.Play();
        if (_ps != null)
        {
            _ps.Play();
        }
        SpawnGoalPrefabs();
        AnimOpenBox();
    }
    private void SpawnGoalPrefabs()
    {
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

                CloseBoxAlphaEffect(targetBox, newObj);
                GetGoalItem(newObj);
                OpenBoxClick(newObj, targetBox);
                index++;
            }
        }
    }
    private void GetGoalItem(GameObject obj)
    {
        if(obj.TryGetComponent<GoalItem>(out var goalItem))
        {
            int id = (int)goalItem.GetColor();
            int totalCount = _tilePool.TileCount[id];
            goalItem.Initialize(totalCount / 3);
        }
    }
    private void OpenBoxClick(GameObject obj,Transform target)
    {
        if(obj.TryGetComponent<GoalItemClickHandler>(out var goalItemClickHandler))
        {
            if (_openBox.Contains(target))
            {
                goalItemClickHandler.Set(this);
            }
        }
    }
    private void CloseBoxAlphaEffect(Transform target,GameObject obj)
    {
        AlphaAdjust alpha = obj.GetComponent<AlphaAdjust>();
        if (closedBoxList.Contains(target))
        {
            alpha.AlphaAdjustable(obj, 0.5f);
        }
    }
    
    private void RemoveAlpha(GameObject obj)
    {
        if(obj.TryGetComponent<IAlphaAdjustable>(out var alpha))
        {
            alpha.SetAlpha(1f);
        }
    }
    private void RemoveOutLine(GameObject obj)
    {
       if(obj.TryGetComponent<OutLineDrawer>(out var outLineDrawer))
        {
            outLineDrawer.DisableOutLine();
        }
    }
    private void AnimOpenBox()
    {
        for (int i = 0; i < _openBox.Length; i++)
        {
            Transform box = _openBox[i];
            if (box.childCount == 0) continue;

            GameObject child = box.GetChild(0).gameObject;
            AnimNewGoalItem(child, i * _delay);
        }
    }

    private void AnimNewGoalItem(GameObject child, float delay = 0.1f)
    {
        child.transform.localPosition = Vector3.zero;
        child.transform.localScale = new Vector3(1.2f, 0.6f, 1f); // squash başlangıcı

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.Append(child.transform.DOScale(Vector3.one * 1.1f, _spawnDuration * 0.4f).SetEase(Ease.OutBack)); // büyüme
        seq.Append(child.transform.DOScale(Vector3.one * 0.95f, _spawnDuration * 0.2f).SetEase(Ease.InOutSine)); // geri sıkışma
        seq.Append(child.transform.DOScale(Vector3.one, _spawnDuration * 0.2f).SetEase(Ease.OutSine)); // normalize
        seq.AppendCallback(() =>
        {
            child.transform.DOScale(Vector3.one * _animSize, _loopDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        });


        if (child.TryGetComponent<OutLineDrawer>(out var outlineDrawer))
        {
            outlineDrawer.DrawOutLines(child, _outLineColor, _outLineSize); 
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

    public void OnGoalItemClicked(GameObject clickedObject) // Goal Item tıklama
    {
        if(_launcherManager.HasEmptyBox(out int emptyIndex))
        {
            _signalBus.Fire<ClickSoundSignals>();
            clickedObject.transform.SetParent(_launcherManager.GetBoxTransform(emptyIndex));
            ClickedAnimation(clickedObject,emptyIndex);
            RemoveOutLine(clickedObject);
            DOTween.Kill(clickedObject);

        }
        else if (_launcherManager.HasFullBox())
        {
            _signalBus.Fire<ClickSoundSignals>();
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
                CheckAndMoveCloseBox();

            });
    } // Tıklama anim

    private void CheckAndMoveCloseBox()
    {
        for (int i = 0; i < _openBox.Length; i++)
        {
            Transform openBox = _openBox[i];

            if (openBox.childCount == 0)
            {
                Transform closeBox = _closeBox[i];

                if (closeBox.childCount > 0)
                {
                    GameObject goalItem = closeBox.GetChild(0).gameObject;

                    goalItem.transform.SetParent(openBox);

                   // TrailRenderer trail = _trailPool.GetTrial();
                   //trail.transform.SetParent(goalItem.transform);

                    //Debug.Log("TrailPos " + trail.transform.position);

                    goalItem.transform.DOMove(openBox.position, 0.35f).SetEase(Ease.InOutQuad)
                        .OnComplete(() =>
                        {
                            goalItem.transform.localPosition = Vector3.zero;
                            goalItem.transform.localScale = Vector3.one;

                            //_trailPool.ReturnTrail(trail);
                            AnimNewGoalItem(goalItem);
                        });

                    RemoveAlpha(goalItem);
                    OpenBoxClick(goalItem, openBox);
                    CascadeCloseBox(closeBox,i);    
                }
            }
        }
    }

    private void CascadeCloseBox(Transform upperCloseBox, int columnIndex)
    {
        // Sınır kontrolü
        if (columnIndex < 0 || columnIndex >= _closeBox.Length - _openBox.Length)
            return;

        int lowerIndex = columnIndex + _openBox.Length;
        Transform lowerCloseBox = _closeBox[lowerIndex];

        if (lowerCloseBox.childCount > 0)
        {
            GameObject goalItem = lowerCloseBox.GetChild(0).gameObject;

            goalItem.transform.SetParent(upperCloseBox);
            goalItem.transform.position = lowerCloseBox.position;

            goalItem.transform.DOMove(upperCloseBox.position, 0.35f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    goalItem.transform.localScale = Vector3.one;
                    goalItem.transform.localPosition = Vector3.zero;
                });
        }
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
