using System.IO;
using FourDimensionalSpace;
using Newtonsoft.Json;
using UnityEngine;

namespace ShapeMetaData
{
	public static class ShapeFactory
	{	
		public static Shape CreateShape(ShapeType shapeType)
		{
			string path = Path.Combine(Application.streamingAssetsPath, "ShapeMetaData", shapeType + ".json");

			// пока редактируется генератор - закомментил, чтобы каждый запуск обновлялось
			//if (!File.Exists(path))
			MetaDataGenerator.GenerateDataFile(shapeType);

			return JsonConvert.DeserializeObject<Shape>(LoadString(path));
		}

		private static string LoadString(string url)
		{
			if (Application.platform == RuntimePlatform.OSXEditor ||
			    Application.platform == RuntimePlatform.OSXPlayer)
				url = "file://" + url;
			
			return new WWW(url).text;
		}
		
	}
}
