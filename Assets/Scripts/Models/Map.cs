using UnityEngine;

namespace Models
{
	public class Map {

		public string MapName;
		public float MapSize;
 
		public Map() {
		}

		public Map(string mapName, float mapSize) {
			MapName = mapName;
			MapSize = mapSize;
		}
		
	}
}
