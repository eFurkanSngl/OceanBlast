using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;
using Zenject;

public class Tile : MonoBehaviour
{
    public enum TileColor { Yellow = 0, Blue = 1, Red = 2, White = 3 };

    private SpriteRenderer _sr;
    public TileColor tileColor { get; private set; }
    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
    }
   
    public void Initialize(TileColor color)
    {
        tileColor = color;
        SpawnEffect();
    }

    private void SpawnEffect()
    {
        transform.localScale = Vector3.zero;

        transform.DOScale(Vector3.one, 0.25f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // Pulse Scale (nefes alma efekti)
                transform.DOScale(Vector3.one * 1.1f, 0.5f)
                    .SetEase(Ease.InOutSine)
                    .SetLoops(3, LoopType.Yoyo);
            });

        if (_sr != null)
        {
            Color originColor = _sr.color;
            Color brightColor = originColor * 1.5f;
            brightColor.a = originColor.a;

            _sr.DOColor(brightColor, 0.25f)
                .OnComplete(() =>
                {
                    _sr.DOColor(originColor, 0.25f);
                });
        }
    }
    public void ResetTile() => tileColor = (TileColor)(-1);
    public int GetId()
    {
        return (int)tileColor;
    }
}

