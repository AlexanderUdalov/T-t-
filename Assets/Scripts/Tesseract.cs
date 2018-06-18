using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FourDimensionalSpace
{
    public class Tesseract : Shape
    {
        public float SideLength;

        public Tesseract(float sideLength = 1f)
        {
            Vertices = new Vertex[16];
            AdjacencyMatrix = new byte[16,16];
            SideLength = sideLength;
        }

        public override void Initialize()
        {
            int signX = 1, signY = 1, signZ = 1, signW = 1;

            //Vertices initializing
            for (int i1 = 0; i1 < 2; i1++)
            {
                for (int i2 = 0; i2 < 2; i2++)
                {
                    for (int i3 = 0; i3 < 2; i3++)
                    {
                        for (int i4 = 0; i4 < 2; i4++)
                        {
                            Vertices[i1 * 8 + i2 * 4 + i3 * 2 + i4] = new Vertex(
                                SideLength * signX, 
                                SideLength * signY, 
                                SideLength * signZ, 
                                SideLength * signW);

                            signX *= -1;
                        }
                        signY *= -1;
                    }
                    signZ *= -1;
                }
                signW *= -1;
            }

            //Matrix initializing
            for (int i = 0; i < 16; i += 4)
            {
                AdjacencyMatrix[i, i + 1] = 1;
                AdjacencyMatrix[i + 1, i + 3] = 1;
                AdjacencyMatrix[i + 3, i + 2] = 1;
                AdjacencyMatrix[i + 2, i] = 1;
            }
            for (int i = 0; i < 4; i++)
            {
                AdjacencyMatrix[i, i + 4] = 1;
            }
            for (int i = 8; i < 12; i++)
            {
                AdjacencyMatrix[i, i + 4] = 1;
            }
            for (int i = 0; i < 8; i++)
            {
                AdjacencyMatrix[i, i + 8] = 1;
            }
        }
    }
}
