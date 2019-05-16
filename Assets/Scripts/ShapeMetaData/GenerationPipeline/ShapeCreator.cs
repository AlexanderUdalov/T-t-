using System.Collections.Generic;
using FourDimensionalSpace;

namespace ShapeMetaData
{
    public class ShapeCreator
    {
        private static readonly Dictionary<ShapeType, int[]> ShapeData = new Dictionary<ShapeType, int[]>
        {
            [ShapeType.Cell5]   = new []{ 5, 10, 10, 5 },
            [ShapeType.Cell8]   = new []{ 16, 32, 24, 8 },
            [ShapeType.Cell16]  = new []{ 8, 24, 32, 16 },
            [ShapeType.Cell24]  = new []{ 24, 96, 96, 24 },
            [ShapeType.Cell120] = new []{ 600, 1200, 720, 120 },
            [ShapeType.Cell600] = new []{ 120, 720, 1200, 600 }
        };
        
        public Shape GetShape(ShapeType shapeType)
        {
            var shapeData = ShapeData[shapeType];
            
            var shape = new Shape(shapeData[0], shapeData[1], shapeData[2], shapeData[3]);
            shape.ShapeType = shapeType;

            return shape;
        }

        public Shape GetShape(int numberOfVertices, int numberOfEdges, int numberOfFaces, int numberOfCells)
        {
            var shape = new Shape(numberOfVertices, numberOfEdges, numberOfFaces, numberOfCells);
            shape.ShapeType = ShapeType.Custom;

            return shape;
        }
    }
}