using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FourDimensionalSpace;
using Newtonsoft.Json;
using UnityEngine;

namespace ShapeMetaData
{
    public class MetaDataGenerator
    {
        private readonly ShapeCreator _shapeCreator = new ShapeCreator();

        private readonly IGenerator[] _generationPipeline = 
        {
            new VerticesGenerator(),
            new EdgesGenerator(),
            new FacesGenerator(),
            new CellsGenerator(),
        };
        
        public void GenerateDataFile(ShapeType shapeType)
        {
            Shape shape = _shapeCreator.GetShape(shapeType);

            for (int i = 0; i < _generationPipeline.Length; i++)
            {
                _generationPipeline[i].Execute(shape);
            }

            Debug.Log("Generation finished successfully.");
            string data = JsonConvert.SerializeObject(shape);
            string writePath = Path.Combine(Application.streamingAssetsPath, "ShapeMetaData", shapeType + ".json");
                        
            File.WriteAllText(writePath, data);
        }
    }
}