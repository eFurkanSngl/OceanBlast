using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GoalItem : MonoBehaviour
{
    [Serializable]
    public class GoalItemData
    {
        //public int tileId;
        public Tile.TileColor tileColor;
    }

    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private GoalItemData _goalItemData;

    private int _currentCount;
    public int CurrentCount => _currentCount;
    public TextMeshProUGUI CountText => _countText;
    public bool IsLauncher { get; set; } = false;
    public void Initialize(int count)
    {
        _currentCount = count;
        UpdateText();
    }

    public int GetID() => (int)_goalItemData.tileColor;
    public Tile.TileColor GetColor() => _goalItemData.tileColor;
    private void UpdateText()
    {
        if( _countText != null )
        {
            _countText.text = _currentCount.ToString();
        }
    }

    public void DestroyGoalItem(GameObject obj)
    {
        if(_currentCount == 0)
        {
            GoalItemDestroyAnim(obj, () =>
            {
                Destroy(obj.gameObject);
            });
        }
    }
    private void GoalItemDestroyAnim(GameObject obj, Action onComplete)
    {
        transform.DOKill();

        Image img = GetComponent<Image>();
        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOScale(1.2f, 0.1f).SetEase(Ease.OutBack));
        seq.Join(transform.DOShakePosition(0.2f, 5f, 5, 30, false, true));
        seq.Append(transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack));
        if (img != null)
            seq.Join(img.DOFade(0f, 0.2f));
        seq.OnComplete(() => onComplete?.Invoke());
    }
    public void DecreaseCount(int amount)
    {
        _currentCount -= amount;
        if(_currentCount < 0)
        {
            _currentCount = 0;
        }
        UpdateText();
    }
}
