using UnityEngine;
using UnityEngine.UI;

public class AlphaAdjust : MonoBehaviour, IAlphaAdjustable
{
    private Image _image;
    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public void SetAlpha(float alpha)
    {
        if(_image != null)
        {
            Color color  = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}
