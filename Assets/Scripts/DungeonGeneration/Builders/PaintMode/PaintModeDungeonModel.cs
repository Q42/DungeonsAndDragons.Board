using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;
using UnityEngine;

namespace DungeonGeneration.Builders.PaintMode
{    
    public class PaintModeDungeonModel : DungeonModel {

        [HideInInspector]
        public PaintModeDungeonConfig Config;
        
        [HideInInspector]
        [SerializeField]
        public DungeonModelBuildState State = DungeonModelBuildState.Initial;

        [SerializeField]
        [HideInInspector]
        public List<Cell> Cells = new List<Cell>();

    }
}
