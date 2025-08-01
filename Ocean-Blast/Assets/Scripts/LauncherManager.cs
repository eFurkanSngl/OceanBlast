﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class LauncherManager : MonoBehaviour
{
    [SerializeField] private Transform[] _launcherBox;
    private GoalItem[] _goalItemsInLauncher; // boxlara hangi GoalItem var tutmak için.
    private bool[] _reservedSlot;
    [Inject] private SignalBus _signalBus;
    [Inject] private GridManager _gridManager;
    [SerializeField] private float _placeGoalDuration = 0.08f;
    [SerializeField] private float _mergeAnimDuration = 0.1f;
    private bool _isMerging = false;
    public bool IsMerging => _isMerging;
    private Dictionary<int, bool> _idFireLock = new Dictionary<int, bool>();

    private void Awake()
    {
        _goalItemsInLauncher = new GoalItem[_launcherBox.Length];  // Launcher box elemanı kadar 
        _reservedSlot = new bool[_launcherBox.Length];
    }

    public bool HasEmptyBox(out int index)  // La unher Kutualrından biri boş mu diye bakar
    {
        for(int i = 0; i <_launcherBox.Length; i++)
        {
            if (_goalItemsInLauncher[i] == null && !_reservedSlot[i])
            {
                index = i;
                _reservedSlot[i] = true;
                return true;
            }
        }
        index = -1;
        return false;
    } // laucnhere goalbox atarken

    public void PlaceGoalItem(GoalItem item , int index) // GoalBox'ı ilk boş Launchere at
    {
        item.transform.SetParent(_launcherBox[index]);
        item.transform.localPosition = Vector3.zero;    
        item.transform.localScale = Vector3.one;

        PlaceGoalBoxAnim(item);

        _goalItemsInLauncher[index] = item;
        _reservedSlot[index] = false;
       
        CanvasGroup group = item.GetComponent<CanvasGroup>();
        if(group != null)
        {
            group.blocksRaycasts = false;
        }

        CheckMerge();

        StartCoroutine(FireGoalItemRoutine(item));
    }

    private IEnumerator FireGoalItemRoutine(GoalItem item)
    {
        int id = item.GetID();

        if (!_idFireLock.ContainsKey(id))
            _idFireLock[id] = false;

        while (_idFireLock[id] || _isMerging)
            yield return null;

        _idFireLock[id] = true;

        yield return StartCoroutine(_gridManager.GoalItemMatchRoutine(item));

        _idFireLock[id] = false;
    }

    public bool HasFullBox() // Launcher Dolu mu diye kontrol
    {
        foreach(var goalItem in _goalItemsInLauncher)
        {
            if(goalItem == null) return false;
        }
        return true;
    }

    public void RemoveGoalItem(int index)
    {
        if( index >= 0 && index < _goalItemsInLauncher.Length)
        {
            if(_goalItemsInLauncher[index] != null)
            {
                Destroy(_goalItemsInLauncher[index].gameObject);
                _goalItemsInLauncher[index] = null;
            }
        }
    }

    private void CheckMerge()
    {
        for (int i = 0; i < _goalItemsInLauncher.Length; i++)
        {
            if (_goalItemsInLauncher[i] == null) continue;

            int matchId = _goalItemsInLauncher[i].GetID();
            int matchCount = 1;
            List<int> matchedList = new List<int> { i };

            for (int y = i + 1; y < _goalItemsInLauncher.Length; y++)
            {
                if (_goalItemsInLauncher[y] != null && _goalItemsInLauncher[y].GetID() == matchId)
                {
                    matchCount++;
                    matchedList.Add(y);
                }

                if (matchCount == 3)
                {
                    Debug.Log($"Merge yapılabilir! ID: {matchId}, indexler: {string.Join(", ", matchedList)}");
                    MergeAnim(matchedList);
                    return;
                }
            }
        }
    }
    private void MergeAnim(List<int> matchedList)
    {
        _isMerging = true;
        matchedList.Sort();
        int centerIndex = matchedList[1];
        GoalItem centerItem = _goalItemsInLauncher[centerIndex];

        for (int i = 0; i < matchedList.Count; i++)
        {
            int currentIndex = matchedList[i];
            if (currentIndex == centerIndex)
                continue;

            GoalItem itemMerge = _goalItemsInLauncher[currentIndex];
            if (itemMerge == null) continue;

            Transform targetTrans = _launcherBox[centerIndex];
            Transform itemTrans = itemMerge.transform;

            Sequence seq = DOTween.Sequence();
            seq.Append(itemTrans.DOMove(targetTrans.position, _mergeAnimDuration).SetEase(Ease.InOutSine));
            seq.Join(itemTrans.DOScale(1.4f, _mergeAnimDuration));
            seq.Append(itemTrans.DOScale(0.3f, _mergeAnimDuration));
            seq.AppendCallback(() =>
            {
                itemTrans.DOKill();
                Destroy(itemMerge.gameObject);
                _goalItemsInLauncher[currentIndex] = null;
            });
        }
            centerItem.transform.DOScale(1.5f, _mergeAnimDuration).SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                centerItem.transform.DOScale(Vector3.one,_mergeAnimDuration).SetEase(Ease.InOutSine);

                ItemTextMerge(matchedList);

                _signalBus.Fire<MergeSoundBus>();
                _isMerging = false;
            });
    }
    private void ItemTextMerge(List<int> matchedList)
    {
        matchedList.Sort();
        int centerIndex = matchedList[1];
        GoalItem centerItem = _goalItemsInLauncher[centerIndex];
        if (centerItem == null)
        {
            return;
        }

        int centerID = centerItem.GetID();
        int totalCount = 0;

        HashSet<int> uniqueIndices = new HashSet<int>(matchedList);
        foreach (int index in uniqueIndices)
        {
            var item = _goalItemsInLauncher[index];
            if (item == null) continue;
           
            if (item.GetID() == centerID)
            {
                totalCount += item.CurrentCount;
            }
        }
        centerItem.Initialize(totalCount);
    }
    private void PlaceGoalBoxAnim(GoalItem item)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(item.transform.DOScale(new Vector3(1.3f, 0.7f, 1f), _placeGoalDuration).SetEase(Ease.OutQuad));  // squash
        seq.Append(item.transform.DOScale(new Vector3(0.9f, 1.1f, 1f), _placeGoalDuration).SetEase(Ease.OutQuad));  // stretch
        seq.Append(item.transform.DOScale(Vector3.one, _placeGoalDuration).SetEase(Ease.OutQuad));
    }

    public Transform GetBoxTransform(int index)
    {
        return _launcherBox[index];
    }  // GoalItem mı taşımak için Box'un konumunu alıyoruz
}
