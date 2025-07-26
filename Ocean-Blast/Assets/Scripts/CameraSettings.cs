using UnityEngine;
using Cinemachine;
using System;
public class BlastCameraSetup : MonoBehaviour
{
    [SerializeField] private float cameraDepth = -10f;
    [SerializeField] private float tiltAngle = 15f;
    [SerializeField] private float _camSizeValueX = 2.7f;
    [SerializeField] private float _camSizeValueY = 4f;
    [SerializeField] private float _camOrtagraphicSizeX = 1.2f;
    [SerializeField] private float _camOrtagraphicSizeY = 1.2f;

    private Camera _cam;
    private void Awake()
    {
        _cam = Camera.main;
    }
    private void SetupCamera(int gridX , int gridY)
    {
        if (_cam == null) return;

        _cam.orthographic = true;

        // Slight tilt on X to fake 3D depth
        _cam.transform.rotation = Quaternion.Euler(tiltAngle, 0, 0);

        // Position camera to center the grid
        _cam.transform.position = new Vector3(gridX / _camSizeValueX, gridY / _camSizeValueY, cameraDepth);

        // Auto adjust orthographic size
        float screenRatio = (float)Screen.width / Screen.height;
        float targetRatio = (float)gridX / gridY;

        if (screenRatio >= targetRatio)
            _cam.orthographicSize = gridY / _camOrtagraphicSizeY;
        else
            _cam.orthographicSize = (gridX / screenRatio) / _camOrtagraphicSizeX;

        Debug.Log("working");
    }
    private void OnEnable()
    {
        RegisterEvents();
    }
    private void OnDisable()
    {
        UnRegisterEvents();
    }
    private void RegisterEvents() => GridManager.GridManagerEvents += SetupCamera;
    private void UnRegisterEvents() => GridManager.GridManagerEvents -= SetupCamera;
}
