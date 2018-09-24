using System;
using System.Collections.Generic;
using ShapeRotation;
using Teta;
using UnityEngine;
using UnityEngine.UI;
using Plane = FourDimensionalSpace.Plane;

public class InputHelper : MonoBehaviour 
{
    private struct SliderData
    {
        public Slider Slider { get; set; }
        public Plane Plane { get; set; }
    }
    
    public ShapeController ShapeController;
    public Camera Camera;

    private float _Fov;
    public float FOV {
        get
        {
            return FOV;
        }
        set
        {
            Camera.fieldOfView = value;
            _Fov = value;
        }
    }

    public Slider XoY;
    public Slider XoZ;
    public Slider XoW;
    public Slider YoZ;
    public Slider YoW;
    public Slider ZoW;

    private List<SliderData> _planeToSlider;

    private void Awake()
    {
        _planeToSlider = new List<SliderData>
        {
            new SliderData { Plane = Plane.XoY, Slider = XoY },
            new SliderData { Plane = Plane.XoZ, Slider = XoZ },
            new SliderData { Plane = Plane.XoW, Slider = XoW },
            new SliderData { Plane = Plane.YoZ, Slider = YoZ },
            new SliderData { Plane = Plane.YoW, Slider = YoW },
            new SliderData { Plane = Plane.ZoW, Slider = ZoW }
        };
    }

    private void Update()
    {
        foreach (var data in _planeToSlider)
        {
            if (data.Slider.value != 0) 
                ShapeController.RotateShape(data.Slider.value, data.Plane);
        }
    }
}
