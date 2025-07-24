using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherManager : MonoBehaviour
{
    [SerializeField] private Transform[] _launcherBox;
    private GoalItem[] _goalItemsInLauncher; // boxlara hangi GoalItem var tutmak için.
    private bool[] _reservedSlot;
    private void Awake()
    {
        _goalItemsInLauncher = new GoalItem[_launcherBox.Length];  // Launcher box elemaný kadar 
        _reservedSlot = new bool[_launcherBox.Length];
    }

    public bool HasEmptyBox(out int index)  // La unher Kutualrýndan biri boþ mu diye bakar
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

    public void PlaceGoalItem(GoalItem item , int index) // GoalBox'ý ilk boþ Launchere at
    {
        item.transform.SetParent(_launcherBox[index]);
        item.transform.localPosition = Vector3.zero;    
        item.transform.localScale = Vector3.one;

        PlaceGoalBoxAnim(item);

        _goalItemsInLauncher[index] = item;

        _reservedSlot[index] = false;
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

    private void PlaceGoalBoxAnim(GoalItem item)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(item.transform.DOScale(new Vector3(1.3f, 0.7f, 1f), 0.08f).SetEase(Ease.OutQuad));  // squash
        seq.Append(item.transform.DOScale(new Vector3(0.9f, 1.1f, 1f), 0.08f).SetEase(Ease.OutQuad));  // stretch
        seq.Append(item.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutQuad));
    }

    public Transform GetBoxTransform(int index)
    {
        return _launcherBox[index];
    }  // GoalItem mý taþýmak için Box'un konumunu alýyoruz
}
