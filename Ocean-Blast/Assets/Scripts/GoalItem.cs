using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
