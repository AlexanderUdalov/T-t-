using System.Collections;
using System.Collections.Generic;
using MF;
using ShapeMetaData;
using ShapeRendering;
using ShapeRotation;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    private const float LerpingTime = 0.4f;
    public Gradient[] BackgroundGradients;
    public Transform[] Anchors;
    public GradientBackground GradientBackground;
    public Transform Camera;

    private int _currentIndex = 0;
    private int _nextIndex = 0;
    private Coroutine _lerpingCoroutine;
    private IRenderingController _renderingController;

    private IEnumerator Start()
    {
        var shapeFactory = new ShapeFactory();
        yield return StartCoroutine(shapeFactory.CreateShape(this, ShapeType.Cell24));
        var cell24 = shapeFactory.GetCreatedShape();


        var cell24controller = new RenderingController(cell24)
            .AddRenderer(new DotsShapeRenderer(Anchors[0]))
            .AddRenderer(new LinesShapeRenderer(Anchors[0]))
            //.AddRenderer(new CellsShapeRenderer());
            .AddRenderer(new FacesShapeRenderer(Anchors[0]));

        cell24controller.BuildShapeView();


        yield return StartCoroutine(shapeFactory.CreateShape(this, ShapeType.Cell5));
        var cell5 = shapeFactory.GetCreatedShape();


        var cell5controller = new RenderingController(cell5)
            .AddRenderer(new DotsShapeRenderer(Anchors[1]))
            .AddRenderer(new LinesShapeRenderer(Anchors[1]))
            //.AddRenderer(new CellsShapeRenderer());
            .AddRenderer(new FacesShapeRenderer(Anchors[1]));

        cell5controller.BuildShapeView();


        yield return StartCoroutine(shapeFactory.CreateShape(this, ShapeType.Cell8));
        var cell8 = shapeFactory.GetCreatedShape();


        var cell8controller = new RenderingController(cell8)
            .AddRenderer(new DotsShapeRenderer(Anchors[2]))
            .AddRenderer(new LinesShapeRenderer(Anchors[2]))
            //.AddRenderer(new CellsShapeRenderer());
            .AddRenderer(new FacesShapeRenderer(Anchors[2]));

        cell8controller.BuildShapeView();


        yield return StartCoroutine(shapeFactory.CreateShape(this, ShapeType.Cell16));
        var cell16 = shapeFactory.GetCreatedShape();


        var cell16controller = new RenderingController(cell16)
            .AddRenderer(new DotsShapeRenderer(Anchors[3]))
            .AddRenderer(new LinesShapeRenderer(Anchors[3]))
            //.AddRenderer(new CellsShapeRenderer());
            .AddRenderer(new FacesShapeRenderer(Anchors[3]));

        cell16controller.BuildShapeView();


        yield return StartCoroutine(shapeFactory.CreateShape(this, ShapeType.Cell120));
        var cell120 = shapeFactory.GetCreatedShape();


        var cell120controller = new RenderingController(cell120)
            .AddRenderer(new DotsShapeRenderer(Anchors[4]))
            .AddRenderer(new LinesShapeRenderer(Anchors[4]))
            //.AddRenderer(new CellsShapeRenderer());
            .AddRenderer(new FacesShapeRenderer(Anchors[4]));

        cell120controller.BuildShapeView();


        yield return StartCoroutine(shapeFactory.CreateShape(this, ShapeType.Cell600));
        var cell600 = shapeFactory.GetCreatedShape();


        var cell600controller = new RenderingController(cell600)
            .AddRenderer(new DotsShapeRenderer(Anchors[5]))
            .AddRenderer(new LinesShapeRenderer(Anchors[5]))
            //.AddRenderer(new CellsShapeRenderer());
            .AddRenderer(new FacesShapeRenderer(Anchors[5]));

        cell600controller.BuildShapeView();
    }

    public void Increment()
    {
        if (_lerpingCoroutine == null)
        {
            _nextIndex++;
            if (_nextIndex >= BackgroundGradients.Length)
                _nextIndex = 0;

            _lerpingCoroutine = StartCoroutine(LerpBackground());

            Debug.Log(_nextIndex);
        }
    }

    public void Decrement()
    {
        if (_lerpingCoroutine == null)
        {
            _nextIndex--;
            if (_nextIndex < 0)
                _nextIndex = BackgroundGradients.Length - 1;

            Debug.Log(_nextIndex);
            _lerpingCoroutine = StartCoroutine(LerpBackground());
        }
    }

    private IEnumerator LerpBackground()
    {

        Debug.Log("pisos");
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

            Camera.rotation = Quaternion.Euler(0f, 0f.LerpEaseQuad(LerpingTime, elapsedTime / LerpingTime, LerpingTime)
                 * 60 + (_currentIndex * 60), 0f);

            yield return null;
        }

        _lerpingCoroutine = null;
        _currentIndex = _nextIndex;
    }
}
