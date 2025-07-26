using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class OutLineDrawer : MonoBehaviour, IOutLineDrawable
{
    private Outline _outLine;
    private void Awake()
    {
        _outLine = GetComponent<Outline>();
        _outLine.enabled = false;
    }

    public void DrawOutLine(Color color, Vector2 effectDistance)
    {
        if( _outLine != null )
        {
           _outLine.effectColor = new Color(color.r,color.g,color.b,0f);
            _outLine.effectDistance = effectDistance;
            _outLine.enabled = true;

            DOTween.To(() => _outLine.effectColor.a,
                a => _outLine.effectColor = new Color(color.r, color.g, color.b, a),
                1f,
                0.25f);
        }
    }

    public void DrawOutLines(GameObject obj,Color color,Vector3 outLineSize)
    {
        if(obj.TryGetComponent<IOutLineDrawable>(out var outLine))
        {
            outLine.DrawOutLine(color,outLineSize);
        }
    }

    public void DisableOutLine()
    {
        _outLine.enabled=false;
    }
}