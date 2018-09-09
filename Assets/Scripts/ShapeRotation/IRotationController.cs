using FourDimensionalSpace;

namespace ShapeRotation
{
    public interface IRotationController
    {
        void Rotate(float angle, Plane plane);
        void SetShapeData(Shape shape);
    }
}