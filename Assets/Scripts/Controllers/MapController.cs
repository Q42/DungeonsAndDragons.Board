using Datastores;
using Generators;
using UnityEngine;

namespace Controllers
{
    public class MapController : MonoBehaviour
    {
        public FogOfWarController FogOfWar;

//		private MapDatastore _mapDatastore;
//
//		private void Awake()
//		{
//			_mapDatastore = new MapDatastore();
//		}
//
//		private void Start () {
//			_mapDatastore.SubscribeToMaps();
//			_mapDatastore.SaveNewMap("TestMap", 12.0f);
//		}

        private void Start()
        {
            var fogOfWarMesh = GridMeshGenerator.GenerateGrid(10, 10, new[]
            {
                28, 29, 30, 31, 32, 39, 40, 41, 42, 43
            });
            
        }
    }
}