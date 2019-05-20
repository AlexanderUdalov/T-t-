using System.Collections.Generic;
using UnityEngine;
using Plane = FourDimensionalSpace.Plane;

class OuterViewInputController : IInputController
{
    public float Speed { get; set; }

    private Vector2 _touchStart;
    private float _speed2Rotation;

    public OuterViewInputController(float speed2Rotation)
    {
        _speed2Rotation = speed2Rotation;
    }
    
    public List<ShapeRotationData> GetInput()
    {
        var resultList = new List<ShapeRotationData>();

        resultList.AddRange(PlayerMobileInputRotation());
        resultList.AddRange(PlayerKeyboardInputRotation());
        resultList.AddRange(SpeedParameterRotation());

        return resultList;
    }

    private List<ShapeRotationData> PlayerKeyboardInputRotation()
    {
        var x = -Input.GetAxis("Vertical");
        var y = Input.GetAxis("Horizontal");
        
        var result = new List<ShapeRotationData>
        {
            new ShapeRotationData {Angle = x * _speed2Rotation, Plane = Plane.YoZ},
            new ShapeRotationData {Angle = y * _speed2Rotation, Plane = Plane.XoZ}
        };

        return result;
    }
    
    private List<ShapeRotationData> PlayerMobileInputRotation()
    {
        var result = new List<ShapeRotationData>();
        
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

            result.Add(new ShapeRotationData {Angle = x * Speed, Plane = Plane.YoZ});
            result.Add(new ShapeRotationData {Angle = y * Speed, Plane = Plane.XoZ});
        }

        return result;
    }

    private List<ShapeRotationData> SpeedParameterRotation()
    {
        var result = new List<ShapeRotationData>
        {
            new ShapeRotationData {Angle = Speed, Plane = Plane.XoW},
            new ShapeRotationData {Angle = Speed, Plane = Plane.YoW},
            new ShapeRotationData {Angle = Speed, Plane = Plane.ZoW}
        };

        return result;
    }
}