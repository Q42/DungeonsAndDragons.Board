using DungeonArchitect;
using UnityEngine;

namespace DungeonGeneration.Builders.PaintMode
{
	public class PaintModeDungeonConfig : DungeonConfig {

		/// <summary>
		/// This dungeon builder works on a grid based system and required modular mesh assets 
		/// to be placed on each cell (floors, walls, doors etc). This important field specifies the size
		/// of the cell to use. This size is determined by the art asset used in the dungeon theme 
		/// designed by the artist. In the demo, we have a floor mesh that is 400x400. The height
		/// of a floor is chosen to be 200 units as the stair mesh is 200 units high. Hence the
		/// defaults are set to 400x400x200. You should change this to the dimension of the modular
		/// asset your designer has created for the dungeon
		/// </summary>
		[Tooltip(@"This dungeon builder works on a grid based system and required modular mesh assets to be placed on each cell (floors, walls, doors etc). This important field specifies the size of the cell to use. This size is determined by the art asset used in the dungeon theme designed by the artist. In the demo, we have a floor mesh that is 400x400. The height of a floor is chosen to be 200 units as the stair mesh is 200 units high. Hence the defaults are set to 400x400x200. You should change this to the dimension of the modular asset your designer has created for the dungeon")]
		public Vector3 GridCellSize = new Vector3(1, 1, 1);
		
		/// <summary>
		/// The extra width to apply to one side of a corridor
		/// </summary>
		[Tooltip(@"The extra width to apply to one side of a corridor")]
		public int CorridorPadding = 1;

		/// <summary>
		/// Flag to apply the padding on both sides of the corridor
		/// </summary>
		[Tooltip(@"Flag to apply the padding on both sides of the corridor")]
		public bool CorridorPaddingDoubleSided;
		
	}
}
