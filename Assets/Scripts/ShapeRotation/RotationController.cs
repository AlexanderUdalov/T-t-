using FourDimensionalSpace;
using Plane = FourDimensionalSpace.Plane;

namespace ShapeRotation
{
    public class RotationController : IRotationController
    {
        private Shape _shape;
        
        public void SetShapeData(Shape shape)
        {
            _shape = shape;
        }

        public RotationController(Shape shape)
        {
            _shape = shape;
        }
        
        public void Rotate(float angle, Plane plane)
        {
            _shape.Rotate(angle, plane);
        }
    }
}