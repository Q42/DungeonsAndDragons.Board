using System.Collections.Generic;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;
using DungeonGeneration.Gizmos;
using UnityEditor;
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
        
        private Dungeon _dungeon;

        [Header("Paint mode")]
        [Tooltip("Enable paint mode")]
        public bool PaintModeEnabled;

        private void Awake()
        {
            model = GetComponent<PaintModeDungeonModel>();
            _paintModel = (PaintModeDungeonModel) model;

            config = GetComponent<PaintModeDungeonConfig>();
            _paintConfig = (PaintModeDungeonConfig) config;
            
            _dungeon = GetComponent<Dungeon>();
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += ManageStateChange;

            PopulateToolData();
        }
        
        private void OnApplicationQuit()
        {
            DestroyPaintingGizmo();
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
            PaintModeEnabled = false;

            if (!_paintGizmo) return;

            DestroyImmediate(_paintGizmo);
        }

        private void GeneratePaintingGizmo()
        {
            if (_paintGizmo) return;
            
            _paintGizmo = gameObject.AddComponent<PaintModeGizmo>();
            _paintGizmo.Config = _paintConfig;
            _paintGizmo.OnPaint += OnPaint;
        }

        private void OnPaint(Vector3 position)
        {
            _dungeon.AddPaintCell(new IntVector(
                (int) position.x, 
                (int) position.y, 
                (int) position.z), true);
        }

        private void PopulateToolData()
        {
            if (_paintModel.ToolData != null) return;
            
            Undo.RecordObjects(new Object[] {_paintModel}, "Enter Tool Mode");
            _paintModel.ToolData = _paintModel.CreateToolDataInstance();
            Undo.RecordObjects(new Object[] { _paintModel.ToolData }, "Create Tool Data");
        }
        
        private void ManageStateChange(PlayModeStateChange state)
        {
            DestroyPaintingGizmo();
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