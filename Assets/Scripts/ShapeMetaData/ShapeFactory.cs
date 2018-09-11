using System;
using System.Collections;
using System.IO;
using FourDimensionalSpace;
using Newtonsoft.Json;
using UnityEngine;

namespace ShapeMetaData
{
	public static class ShapeFactory
	{
		private static Shape _currentShape;
		
		public static IEnumerator CreateShape(MonoBehaviour coroutineHost, ShapeType shapeType)
		{
			string path = Path.Combine(Application.streamingAssetsPath, "ShapeMetaData", shapeType + ".json");

			// пока редактируется генератор - закомментил, чтобы каждый запуск обновлялось
			//if (!File.Exists(path))
				//MetaDataGenerator.GenerateDataFile(shapeType);
			
			string jsonData = null;
			yield return coroutineHost.StartCoroutine(LoadStringAsset(path, data => jsonData = data));
			
			_currentShape = JsonConvert.DeserializeObject<Shape>(jsonData);
		}

		public static Shape GetCreatedShape()
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
