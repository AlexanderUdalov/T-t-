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
        public float Speed { get; set; }

        private IRotationController _rotationController;
        private IRenderingController _renderingController;

        private bool _rerenderRequired;

        private IEnumerator Start() 
        {
            yield return StartCoroutine(ShapeFactory.CreateShape(this, ShapeType.Cell5));
            var shape = ShapeFactory.GetCreatedShape();
            _rotationController = new RotationController(shape);
            
            _renderingController = new RenderingController(shape)
                //.AddRenderer(new DotsShapeRenderer())
                .AddRenderer(new LinesShapeRenderer())
                .AddRenderer(new CellsShapeRenderer(new ShaderHelper(Player)));
            
            _renderingController.BuildShapeView();
        }

        public void RotateShape(float angle, Plane plane)
        {
            if (Math.Abs(angle) < Mathf.Epsilon)
                return;

            _rerenderRequired = true;
            _rotationController.Rotate(angle * Time.deltaTime, plane);
        }
        
        private void Update()
        {
            GetInputFromButtons();
            
            if (_rerenderRequired)
                _renderingController.ModifyShapeView();
        }
        
        public void ShowShape(ShapeType shapeType) => StartCoroutine(ShowShapeCoroutine(shapeType));
        public void ShowShape(int shapeType) => StartCoroutine(ShowShapeCoroutine((ShapeType) shapeType));

        private IEnumerator ShowShapeCoroutine(ShapeType shapeType)
        {
            yield return StartCoroutine(ShapeFactory.CreateShape(this, shapeType));
            var shape = ShapeFactory.GetCreatedShape();

            _rotationController.SetShapeData(shape);
            _renderingController.SetShapeData(shape);
            _renderingController.BuildShapeView();
        }    
        
        private void GetInputFromButtons()
        {
            //Manual rotating
            //RotateShape(Input.GetAxis("XoY"), Plane.XoY);
            //RotateShape(Input.GetAxis("XoZ"), Plane.XoZ);
            //RotateShape(Input.GetAxis("YoZ"), Plane.YoZ);
            //RotateShape(Input.GetAxis("XoW"), Plane.XoW);
            //RotateShape(Input.GetAxis("YoW"), Plane.YoW);
            //RotateShape(Input.GetAxis("ZoW"), Plane.ZoW);

            //Gameplay input
            Player.Rotate(new Vector3(-Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")));
            RotateShape(Player.forward.x * Speed, Plane.XoW);
            RotateShape(Player.forward.y * Speed, Plane.YoW);
            RotateShape(Player.forward.z * Speed, Plane.ZoW);
        }
    }
}