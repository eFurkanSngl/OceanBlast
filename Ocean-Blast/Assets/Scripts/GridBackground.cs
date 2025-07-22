using UnityEngine;

public class GridBackground : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sr;
    [SerializeField] private float _padding = 0.5f;

    private void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    private void AdjustGridBg(int gridX , int gridY)
    {
        if(_sr != null)
        {
            transform.position = new Vector3(gridX / 2.3f - _padding, gridY / 1.6f - _padding, 1f);
            _sr.size = new Vector3(gridX + _padding, gridY + _padding, 1f);
        }
    }


    private void RegisterEvents() => GridManager.GridManagerEvents += AdjustGridBg;
    private void UnRegisterEvents() => GridManager.GridManagerEvents -= AdjustGridBg;

    private void OnEnable()
    {
        RegisterEvents();
    }

    private void OnDisable()
    {
        UnRegisterEvents();
    }
}
