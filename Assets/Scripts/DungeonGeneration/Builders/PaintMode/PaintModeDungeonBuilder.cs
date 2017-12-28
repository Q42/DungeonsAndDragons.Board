using DungeonArchitect;
using DungeonGeneration.Gizmos;
using UnityEngine;

namespace DungeonGeneration.Builders.PaintMode
{
    /// <inheritdoc />
    /// <summary>
    /// A Dungeon Builder implementation that is a completely empty start for real-time painting.
    /// </summary>
    [ExecuteInEditMode]
    public class PaintModeDungeonBuilder : DungeonBuilder
    {
        private PaintModeDungeonModel _paintModel;
        private PaintModeDungeonConfig _paintConfig;
        private PaintModeGizmo _paintGizmo;

        [Header("Paint mode")]
        [Tooltip("Enable paint mode")]
        public bool PaintModeEnabled;

        private void Awake()
        {
            model = GetComponent<PaintModeDungeonModel>();
            _paintModel = (PaintModeDungeonModel) model;

            config = GetComponent<PaintModeDungeonConfig>();
            _paintConfig = (PaintModeDungeonConfig) config;
        }

        private void Update()
        {
            if (!PaintModeEnabled)
            {
                DestroyPaintingGizmo();
                return;
            }

            GeneratePaintingGizmo();
        }

        private void DestroyPaintingGizmo()
        {
            if (!_paintGizmo) return;

            DestroyImmediate(_paintGizmo);
        }

        private void GeneratePaintingGizmo()
        {
            if (_paintGizmo) return;
            
            _paintGizmo = gameObject.AddComponent<PaintModeGizmo>();
        }

        private void OnApplicationQuit()
        {
            DestroyImmediate(_paintGizmo);
        }

        public override void BuildDungeon(DungeonConfig config, DungeonModel model)
        {
            base.BuildDungeon(config, model);

            _paintModel = (PaintModeDungeonModel) model;
            _paintConfig = (PaintModeDungeonConfig) config;

            BuildCells();
        }

        private void BuildCells()
        {
            _paintModel.Config = _paintConfig;
        }
    }
}