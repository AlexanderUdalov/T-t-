using System;
using UnityEngine;

namespace FourDimensionalSpace
{
    public class Vertex
    {
        public float x { get; private set; }
        public float y { get; private set; }
        public float z { get; private set; }
        public float w { get; private set; }
        
        public Vertex(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public static Vector3 ToThridDimensionalSpace(Vertex currentPoint, Vector4 pointOfView)
        {
            float x = ToOneDimensionalSpace(
                new Vector2(currentPoint.x, currentPoint.w), 
                new Vector2(pointOfView.x, pointOfView.w));

            float y = ToOneDimensionalSpace(
                new Vector2(currentPoint.y, currentPoint.w),
                new Vector2(pointOfView.y, pointOfView.w));

            float z = ToOneDimensionalSpace(
                new Vector2(currentPoint.z, currentPoint.w),
                new Vector2(pointOfView.z, pointOfView.w));

            return new Vector3(x, y, z);
        }

        private static float ToOneDimensionalSpace(Vector2 currentPoint, Vector2 pointOfView)
        {
            // tg(alpha), where alpha - angle between (currentPoint; pointOfView) and OX-axis
            float k = (pointOfView.y - currentPoint.y) / (pointOfView.x - currentPoint.x);
            if (pointOfView.y <= 0 && currentPoint.y <= 0)
            {
                if (pointOfView.y >= currentPoint.y)
                    return pointOfView.x - pointOfView.y / k;
                else
                    return currentPoint.x - currentPoint.y / k;
            }
            else
            {
                if (pointOfView.y >= currentPoint.y)
                    return currentPoint.x - currentPoint.y / k;
                else
                    return pointOfView.x - pointOfView.y / k;
            }
        }
        
        public static void Rotate(ref Vertex vertex, float [,] rotationMatrix)
        {
            if (rotationMatrix.Length != 16)
                throw new ArgumentException(rotationMatrix.Length.ToString());

            float[] inputMatrix = new float[4];

            inputMatrix[0] = vertex.x;
            inputMatrix[1] = vertex.y;
            inputMatrix[2] = vertex.z;
            inputMatrix[3] = vertex.w;

            float[] resultMatrix = new float[4];

            for (int i = 0; i < 4; i++)
            {
                float nextVal = 0;
                for (int k = 0; k < 4; k++)
                {
                    nextVal += inputMatrix[k] * rotationMatrix[k, i];
                }
                resultMatrix[i] = nextVal;
            }

            vertex.x = resultMatrix[0];
            vertex.y = resultMatrix[1];
            vertex.z = resultMatrix[2];
            vertex.w = resultMatrix[3];
        }

        public static float Distance(Vertex v1, Vertex v2) =>
            (float) Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + 
                              (v1.z - v2.z) * (v1.z - v2.z) + (v1.w - v2.w) * (v1.w - v2.w));
        
        public static Vertex operator +(Vertex v1, Vertex v2) => 
            new Vertex(v1.x+v2.x, v1.y+v2.y, v1.z+v2.z, v1.w+v2.w);
        
        public static Vertex operator -(Vertex v1, Vertex v2) => 
            new Vertex(v1.x-v2.x, v1.y-v2.y, v1.z-v2.z, v1.w-v2.w);
        
        public static Vertex operator *(Vertex v, float n) => 
            new Vertex(v.x*n, v.y*n, v.z*n, v.w*n);
        
        public static Vertex operator *(float n, Vertex v) => 
            new Vertex(v.x*n, v.y*n, v.z*n, v.w*n);
        
        public static Vertex operator /(Vertex v, float n) => 
            new Vertex(v.x/n, v.y/n, v.z/n, v.w/n);
    }
}
