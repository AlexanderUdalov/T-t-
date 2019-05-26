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
    public Animator CanvasAnimator;
    public Gradient InitialGradient;
    public Gradient[] BackgroundGradients;
    public Transform[] Anchors;

    public Button IncrementButton, DecrementButton, ShapeButton;

    private GradientBackground _gradientBackground;
    private Transform _camera;
    private int _currentIndex = 0;
    private int _nextIndex = 0;
    private Coroutine _lerpingCoroutine;
    private List<IRenderingController> RenderingControllers = new List<IRenderingController>();

    private void Awake()
    {
        _camera = Camera.main.transform;
        _gradientBackground = _camera.GetComponent<GradientBackground>();
        StartCoroutine(DelayCoroutine(
            CanvasAnimator.runtimeAnimatorController.animationClips[0].length,
            OnSplashPlayed)
            );
    }

    public IEnumerator DelayCoroutine(float delayTime, Action callback)
    {
        yield return new WaitForSeconds(delayTime);
        callback.Invoke();
    }

    public void OnSplashPlayed()
    {
        for (int i = 0; i < Anchors.Length; i++)
        {
            Transform anchor = (Transform)Anchors[i];

            var shapeController = anchor.gameObject.AddComponent<ShapeController>();
            shapeController.Player = _camera;
            shapeController.ShapeType = (ShapeType)i;
        }
        StartCoroutine(LerpFromSplashScreen(
            (Vector3.zero, Anchors[0].localScale),
            (InitialGradient, BackgroundGradients[0]),
            LerpingTime));
    }

    public void OnIncrementClicked()
    {
        if (_lerpingCoroutine == null)
        {
            _nextIndex++;
            if (_nextIndex >= BackgroundGradients.Length)
                _nextIndex = 0;

            _lerpingCoroutine = StartCoroutine(LerpMenu(
                (BackgroundGradients[_currentIndex], BackgroundGradients[_nextIndex]),
                LerpingTime
                ));
        }
    }

    public void OnDecrementClicked()
    {
        if (_lerpingCoroutine == null)
        {
            _nextIndex--;
            if (_nextIndex < 0)
                _nextIndex = BackgroundGradients.Length - 1;

            _lerpingCoroutine = StartCoroutine(LerpMenu(
                (BackgroundGradients[_currentIndex], BackgroundGradients[_nextIndex]),
                LerpingTime
                ));
        }
    }

    public void OnShapeClicked()
    {
        if (_lerpingCoroutine == null)
        {
            IncrementButton.interactable = false;
            DecrementButton.interactable = false;
            ShapeButton.interactable = false;

            CanvasAnimator.SetTrigger("OnClick");

            _lerpingCoroutine = StartCoroutine(
                LerpCameraParameters(
                    (AppSettings.Instance.MenuCameraFOV, AppSettings.Instance.InnerCameraFOV),
                    (_camera.position, Anchors[_currentIndex].position),
                     LerpingTime));

            var controller = Anchors[_currentIndex].GetComponent<ShapeController>();
            controller.enabled = true;
            controller.Speed = AppSettings.Instance.ShapeSpeed;
        }
    }

    public void OnBackToMenuClicked()
    {
        if (_lerpingCoroutine == null)
        {
            CanvasAnimator.SetTrigger("OnClick");

            _lerpingCoroutine = StartCoroutine(
                LerpCameraParameters(
                    (AppSettings.Instance.InnerCameraFOV, AppSettings.Instance.MenuCameraFOV),
                    (_camera.position, Vector3.zero),
                     LerpingTime));

            IncrementButton.interactable = true;
            DecrementButton.interactable = true;
            ShapeButton.interactable = true;

            var controller = Anchors[_currentIndex].GetComponent<ShapeController>();
            controller.enabled = false;
            controller.Speed = 0;
        }
    }

    private IEnumerator LerpFromSplashScreen(
            (Vector3 from, Vector3 to) scale,
            (Gradient from, Gradient to) background,
            float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            LerpBackgroundGradient((background.from, background.to), duration, elapsedTime);
            Anchors[0].transform.localScale = LerpExtensions.LerpEaseQuad(scale.from, scale.to, elapsedTime, duration);

            yield return null;
        }

        _lerpingCoroutine = null;
    }

    private IEnumerator LerpCameraParameters(
        (float from, float to) FOV,
        (Vector3 from, Vector3 to) position,
        float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _camera.position = LerpExtensions.LerpEaseQuad(position.from, position.to, elapsedTime, duration);
            _camera.GetComponent<Camera>().fieldOfView = LerpExtensions.LerpEaseQuad(FOV.from, FOV.to, elapsedTime, duration);
            yield return null;
        }

        _lerpingCoroutine = null;
    }

    private IEnumerator LerpMenu(
        (Gradient from, Gradient to) backgrounds,
        float duration)
    {
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            LerpBackgroundGradient(backgrounds, duration, elapsedTime);

            _camera.rotation = Quaternion.Euler(
                _camera.rotation.x,
                LerpExtensions.LerpAngleEaseQuad(_currentIndex * 60, _nextIndex * 60, elapsedTime, duration),
                _camera.rotation.z);

            yield return null;
        }

        _lerpingCoroutine = null;
        _currentIndex = _nextIndex;
    }

    private void LerpBackgroundGradient((Gradient from, Gradient to) backgrounds, float duration, float elapsedTime)
    {
        var colorKeys = new GradientColorKey[2];
        var alphaKeys = new GradientAlphaKey[2];

        alphaKeys[0] = new GradientAlphaKey();
        alphaKeys[1] = new GradientAlphaKey();

        colorKeys[0] = new GradientColorKey(
            LerpExtensions.LerpEaseQuad(
                backgrounds.from.colorKeys[0].color,
                backgrounds.to.colorKeys[0].color,
                elapsedTime,
                duration
            ), 0);
        colorKeys[1] = new GradientColorKey(
            LerpExtensions.LerpEaseQuad(
                backgrounds.from.colorKeys[1].color,
                backgrounds.to.colorKeys[1].color,
                elapsedTime,
                duration
            ), 1);

        _gradientBackground.Gradient.SetKeys(colorKeys, alphaKeys);
        _gradientBackground.SetDirty();
    }
}
