using System;
using System.Collections;
using System.Collections.Generic;
using MF;
using ShapeMetaData;
using ShapeRendering;
using ShapeRotation;
using Teta;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    private const float LerpingTime = 0.4f;
    public Gradient[] BackgroundGradients;
    public Transform[] Anchors;
    public GradientBackground GradientBackground;
    public Transform Camera;

    public Button IncrementButton, DecrementButton, ShapeButton;

    private int _currentIndex = 0;
    private int _nextIndex = 0;
    private Coroutine _lerpingCoroutine;
    private List<IRenderingController> RenderingControllers = new List<IRenderingController>();

    private void Start()
    {
        for (int i = 0; i < Anchors.Length; i++)
        {
            Transform anchor = (Transform)Anchors[i];

            var shapeController =  anchor.gameObject.AddComponent<ShapeController>();
           shapeController.Player = Camera;
           shapeController.ShapeType = (ShapeType) i;
        }
    }

    public void OnIncrementClicked()
    {
        if (_lerpingCoroutine == null)
        {
            _nextIndex++;
            if (_nextIndex >= BackgroundGradients.Length)
                _nextIndex = 0;

            _lerpingCoroutine = StartCoroutine(LerpBackground());
        }
    }

    public void OnDecrementClicked()
    {
        if (_lerpingCoroutine == null)
        {
            _nextIndex--;
            if (_nextIndex < 0)
                _nextIndex = BackgroundGradients.Length - 1;

            _lerpingCoroutine = StartCoroutine(LerpBackground());
        }
    }

    public void OnShapeClicked()
    {
        if (_lerpingCoroutine == null)
        {
            IncrementButton.interactable = false;
            DecrementButton.interactable = false;
            ShapeButton.interactable = false;
            _lerpingCoroutine = StartCoroutine(
                LerpCameraParameters(
                    (AppSettings.Instance.MenuCameraFOV, AppSettings.Instance.InnerCameraFOV),
                    (Camera.position, Anchors[_currentIndex].position),
                     LerpingTime));

            Anchors[_currentIndex].GetComponent<ShapeController>().Speed = AppSettings.Instance.ShapeSpeed;
        }
    }

    public void OnBackToMenuClicked()
    {
        if (_lerpingCoroutine == null)
        {
            _lerpingCoroutine = StartCoroutine(
                LerpCameraParameters(
                    (AppSettings.Instance.InnerCameraFOV, AppSettings.Instance.MenuCameraFOV),
                    (Camera.position, Vector3.zero),
                     LerpingTime));

            IncrementButton.interactable = true;
            DecrementButton.interactable = true;
            ShapeButton.interactable = true;
            
            Anchors[_currentIndex].GetComponent<ShapeController>().Speed = 0;
        }
    }

    private IEnumerator LerpCameraParameters(
        (float from, float to) FOV,
        (Vector3 from, Vector3 to) position,
        float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < LerpingTime)
        {
            elapsedTime += Time.deltaTime;
            Camera.position = position.from.LerpEaseQuad(position.to, elapsedTime, duration);
            Camera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(FOV.from, FOV.to, elapsedTime / duration);
            yield return null;
        }

        _lerpingCoroutine = null;
    }

    private IEnumerator LerpBackground()
    {
        float elapsedTime = 0;
        while (elapsedTime < LerpingTime)
        {
            elapsedTime += Time.deltaTime;

            var colorKeys = new GradientColorKey[2];
            var alphaKeys = new GradientAlphaKey[2];

            alphaKeys[0] = new GradientAlphaKey();
            alphaKeys[1] = new GradientAlphaKey();

            colorKeys[0] = new GradientColorKey(
                Color.Lerp(
                    BackgroundGradients[_currentIndex].colorKeys[0].color,
                    BackgroundGradients[_nextIndex].colorKeys[0].color,
                    elapsedTime / LerpingTime
                ), 0);
            colorKeys[1] = new GradientColorKey(
                Color.Lerp(
                    BackgroundGradients[_currentIndex].colorKeys[1].color,
                    BackgroundGradients[_nextIndex].colorKeys[1].color,
                    elapsedTime / LerpingTime
                ), 1);

            GradientBackground.Gradient.SetKeys(colorKeys, alphaKeys);
            GradientBackground.SetDirty();

            Camera.rotation = Quaternion.Euler(
                Camera.rotation.x,
                Mathf.LerpAngle(_currentIndex * 60, _nextIndex * 60, elapsedTime / LerpingTime),
                Camera.rotation.z);

            yield return null;
        }

        _lerpingCoroutine = null;
        _currentIndex = _nextIndex;
    }
}
