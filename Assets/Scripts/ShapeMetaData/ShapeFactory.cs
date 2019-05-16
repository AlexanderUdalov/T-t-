using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FourDimensionalSpace;
using Newtonsoft.Json;
using UnityEngine;

namespace ShapeMetaData
{
	public class ShapeTMP
	{
		public ShapeType ShapeType { get; set; }

		public Vertex[] Vertices { get; set; }
		public List<Tuple<int, int>> AdjacencyList { get; set; }
		public List<List<int>> Faces { get; set; }
		public List<List<int>> Cells { get; set; }
	}

	public class ShapeFactory
	{
		private Shape _currentShape;
		
		public IEnumerator CreateShape(MonoBehaviour coroutineHost, ShapeType shapeType)
		{
			string path = Path.Combine(Application.streamingAssetsPath, "ShapeMetaData", shapeType + ".json");

			// пока редактируется генератор - закомментил, чтобы каждый запуск обновлялось
			//if (!File.Exists(path))
			//new MetaDataGenerator().GenerateDataFile(shapeType);
			
			string jsonData = null;
			yield return coroutineHost.StartCoroutine(LoadStringAsset(path, data => jsonData = data));
			
			_currentShape = JsonConvert.DeserializeObject<Shape>(jsonData);
		}

		public Shape GetCreatedShape()
		{
			if (_currentShape == null)
				throw new Exception("Call CreateShape() before taking createdShape.");

			var returnShape = _currentShape;
			_currentShape = null;

			return returnShape;
		}

		private static IEnumerator LoadStringAsset(string url, System.Action<string> result)
		{
			if (Application.platform == RuntimePlatform.OSXEditor || 
			    Application.platform == RuntimePlatform.OSXPlayer)
				url = "file://" + url;

			WWW www = new WWW(url);
			float elapsedTime = 0.0f;

			while (!www.isDone)
			{
				elapsedTime += Time.deltaTime;
				if (elapsedTime >= 10.0f) break;
				yield return null;
			}

			if (!www.isDone || !string.IsNullOrEmpty(www.error))
			{
				Debug.LogError("Load Failed: " + url);
				result(null);    // Pass null result.
				yield break;
			}

			result(www.text); // Pass retrieved result.
		}
		
	}
}
