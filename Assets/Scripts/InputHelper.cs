using FourDimensionalSpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputHelper : MonoBehaviour {

    public TestVisualization VisualizationScript;

    public Slider XoY;
    public Slider XoZ;
    public Slider XoW;
    public Slider YoZ;
    public Slider YoW;
    public Slider ZoW;
    

    private void Update()
    {
        SetInputXoY(XoY.value);
        SetInputXoZ(XoZ.value);
        SetInputXoW(XoW.value);
        SetInputYoZ(YoZ.value);
        SetInputYoW(YoW.value);
        SetInputZoW(ZoW.value);
    }

    public void SetInputXoY(float value) => VisualizationScript.RotateShape(value, Planes.XoY);
    public void SetInputXoZ(float value) => VisualizationScript.RotateShape(value, Planes.XoZ);
    public void SetInputXoW(float value) => VisualizationScript.RotateShape(value, Planes.XoW);
    public void SetInputYoZ(float value) => VisualizationScript.RotateShape(value, Planes.YoZ);
    public void SetInputYoW(float value) => VisualizationScript.RotateShape(value, Planes.YoW);
    public void SetInputZoW(float value) => VisualizationScript.RotateShape(value, Planes.ZoW);
    
}
