using UnityEngine;
using DG.Tweening;
using Zenject;
using System;
using System.Collections;
using UnityEngine.UI;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private Camera _mainCam;
    [Inject] private SignalBus _signalBus;
    private void CamShake()
    {
        _mainCam.transform.DOShakePosition(0.2f, strength: new Vector3(0.1f, 0.1f, 0), vibrato: 10, randomness: 90);
    }
    private void Awake()
    {
        _signalBus.Subscribe<AnimSignalBus>(CamShake);
    }
    
    public void PlayReturnAnim(GameObject obj , Action OnComplete)
    {
        Sequence seq = DOTween.Sequence();
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        seq.Append(obj.transform.DOScale(1.2f, 0.1f).SetEase(Ease.OutBack));
        seq.Append(obj.transform.DOScale(Vector3.zero,0.2f).SetEase(Ease.InBack));

        if(sr!= null)
        {
            seq.Join(sr.DOFade(0, 0.2f));
        }
        seq.OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }
    public void PlayHitEffect(GameObject obj)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(obj.transform.DOScale(1.1f, 0.08f).SetEase(Ease.OutBack));
        seq.Append(obj.transform.DOScale(1f, 0.1f).SetEase(Ease.InBack));
        seq.Join(obj.transform.DOShakeScale(0.2f, 0.1f, 10, 90, false));
    }
    private void OnDestroy()
    {
        _signalBus.Unsubscribe<AnimSignalBus>(CamShake);
    }
}
  