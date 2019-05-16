using System;
using System.Collections;
using ShapeRotation;
using ShapeMetaData;
using ShapeRendering;
using UnityEngine;
using Plane = FourDimensionalSpace.Plane;

namespace Teta
{
    public class ShapeController : MonoBehaviour
    {
        public Transform Player;

        public float Speed
        {
            get => _inputController.Speed;
            set
            {
                if (_inputController != null)
                    _inputController.Speed = value;   
            }
        }

        public float Speed2Rotation;

        private IRotationController _rotationController;
        private IRenderingController _renderingController;
        private IInputController _inputController;

        private readonly ShapeFactory _shapeFactory = new ShapeFactory();
        
        private bool _rerenderRequired;

        private void Awake()
        {
            _inputController = new InnerViewInputController(Player, Speed2Rotation);
        }
        
        private IEnumerator Start() 
        {
            yield return StartCoroutine(_shapeFactory.CreateShape(this, ShapeType.Cell24));
            var shape = _shapeFactory.GetCreatedShape();
            
            _rotationController = new RotationController(shape);
            
            var shaderHelper = new ShaderHelper(Player);
            _renderingController = new RenderingController(shape)
                //.AddRenderer(new DotsShapeRenderer())
                .AddRenderer(new LinesShapeRenderer())
                //.AddRenderer(new CellsShapeRenderer());
                .AddRenderer(new FacesShapeRenderer());
            
            _renderingController.BuildShapeView();
        }

        private void Update()
        {
            var requiredRotations = _inputController.GetInput();
            requiredRotations.ForEach(rotation => RotateShape(rotation.Angle, rotation.Plane));

            if (_rerenderRequired)
            {
                _rerenderRequired = false;
                _renderingController.ModifyShapeView();
            }
        }

        private void RotateShape(float angle, Plane plane)
        {
            if (Mathf.Abs(angle) < Mathf.Epsilon)
                return;

            _rerenderRequired = true;
            _rotationController.Rotate(angle * Time.deltaTime, plane);
        }

        public void ShowShape(ShapeType shapeType) => StartCoroutine(ShowShapeCoroutine(shapeType));
        public void ShowShape(int shapeType) => StartCoroutine(ShowShapeCoroutine((ShapeType) shapeType + 1));

        private IEnumerator ShowShapeCoroutine(ShapeType shapeType)
        {
            yield return StartCoroutine(_shapeFactory.CreateShape(this, shapeType));
            var shape = _shapeFactory.GetCreatedShape();

            _rotationController.SetShapeData(shape);
            _renderingController.SetShapeData(shape);
            _renderingController.BuildShapeView();
        }    
    }
}