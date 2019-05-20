using System.Collections.Generic;
using UnityEngine;
using Plane = FourDimensionalSpace.Plane;

class InnerViewInputController : IInputController
{
    public float Speed { get; set; }
    
    private Transform _player;

    private Vector2 _touchStart;
    private float _speed2Rotation;


    public InnerViewInputController(Transform player, float speed2Rotation)
    {
        _player = player;
        _speed2Rotation = speed2Rotation;
    }

    public List<ShapeRotationData> GetInput()
    {
        var resultList = new List<ShapeRotationData>();

        MovePlayer();        
        resultList.AddRange(MoveForward());

        return resultList;
    }

    private void MovePlayer()
    {
        GetMobileInput();
        _player.Rotate(new Vector3(-Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * Speed * _speed2Rotation);
    }

    private void GetMobileInput()
    {
        if (Input.touchCount > 0)
        {
            float x = 0, y = 0;
            Touch currentTouch = Input.touches[0];
            if (currentTouch.phase == TouchPhase.Began)
            {
                _touchStart = currentTouch.position;
            }
            else
            {
                Vector2 touchEnd = currentTouch.position;
                x = touchEnd.x - _touchStart.x;
                y = touchEnd.y - _touchStart.y;
            }

            _player.Rotate(new Vector3(-y, x) * Speed * _speed2Rotation / 100);
        }
    }
    
    private List<ShapeRotationData> MoveForward()
    {
        var result = new List<ShapeRotationData>
        {
            new ShapeRotationData {Angle = _player.forward.x * Speed, Plane = Plane.XoW},
            new ShapeRotationData {Angle = _player.forward.y * Speed, Plane = Plane.YoW},
            new ShapeRotationData {Angle = _player.forward.z * Speed, Plane = Plane.ZoW}
        };

        return result;
    }
    
//    private void GetInputFromButtons()
//    {
//        Manual rotating
//        RotateShape(Input.GetAxis("XoY"), Plane.XoY);
//        RotateShape(Input.GetAxis("XoZ"), Plane.XoZ);
//        RotateShape(Input.GetAxis("YoZ"), Plane.YoZ);
//        RotateShape(Input.GetAxis("XoW"), Plane.XoW);
//        RotateShape(Input.GetAxis("YoW"), Plane.YoW);
//        RotateShape(Input.GetAxis("ZoW"), Plane.ZoW);
//
//        Gameplay input
//        RotateShape(_player.forward.x * Speed, Plane.XoW);
//        RotateShape(_player.forward.y * Speed, Plane.YoW);
//        RotateShape(_player.forward.z * Speed, Plane.ZoW);
//    }
}