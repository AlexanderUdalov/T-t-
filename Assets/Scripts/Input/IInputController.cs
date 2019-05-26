using System;
using System.Collections.Generic;
using Plane = FourDimensionalSpace.Plane;

public interface IInputController
{
    float Speed { get; set; }
    List<ShapeRotationData> GetInput();
}

public struct ShapeRotationData
{
    public float Angle { get; set; }
    public Plane Plane { get; set; } 
}