#if UNITY_EDITOR
namespace Mapbox.Unity.Tests
{
	using Mapbox.Unity.Map;
	using NUnit.Framework;
	using System.Collections;
	using UnityEngine;
	using UnityEngine.TestTools;

	[TestFixture]
	internal class AbstractMapTests
	{
		GameObject _map;

		[Test]
		public IEnumerator SetUpDefaultMap()
		{
			var go = new GameObject("Map");
			var map = go.AddComponent<AbstractMap>();

			bool initialized = false;
			map.OnInitialized += () =>
			{
				Assert.IsNotNull(map);
				initialized = true;
			};

			yield return new WaitForFixedUpdate();
			map.Initialize(new Mapbox.Utils.Vector2d(37.7749, -122.4194), 15);

			// Esperar a que se complete la inicialización
			yield return new WaitUntil(() => initialized);

			// Limpiar después del test
			Object.DestroyImmediate(go);
		}
	}
}
#endif
