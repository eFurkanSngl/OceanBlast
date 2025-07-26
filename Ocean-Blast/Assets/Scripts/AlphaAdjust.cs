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

    public void AlphaAdjustable(GameObject obj, float alphaValue)
    {
        if(obj.TryGetComponent<IAlphaAdjustable>(out var alpa))
        {
            alpa.SetAlpha(alphaValue);
        }
    }
}
