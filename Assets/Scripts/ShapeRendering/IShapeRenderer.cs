using FourDimensionalSpace;

namespace ShapeRendering
{
    public interface IShapeRenderer
    {
        void SetShapeData(Shape shape);
        void BuildShapeView();
        void ModifyShapeView();
    }
}