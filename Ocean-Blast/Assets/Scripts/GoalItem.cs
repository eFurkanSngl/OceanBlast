using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoalItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _countText;

    private int _tileID;
    private Tile.TileColor _tileColor;
    private int _targetCount;
    private int _currentCount;

    public void Initialize(int id , Tile.TileColor color , int count)
    {
        _tileID = id;
        _tileColor = color;
        _targetCount = count;
        _currentCount = count;
        UpdateText();
    }

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
