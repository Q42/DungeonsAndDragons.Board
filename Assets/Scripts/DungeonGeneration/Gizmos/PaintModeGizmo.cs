using System;
using DungeonGeneration.Builders.PaintMode;
using UnityEditor;
using UnityEngine;

namespace DungeonGeneration.Gizmos
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class PaintModeGizmo : MonoBehaviour
    {
        private GameObject _instance;

        private MeshRenderer _gizmoMeshRenderer;
        private MeshFilter _gizomoMeshFilter;

        public Vector3 GizmoPosition { private get; set; }
        public PaintModeDungeonConfig Config { private get; set; }
        
        public int BrushSize = 1;
        
        public delegate void OnPaintDelegate(Vector3 position);
        public OnPaintDelegate OnPaint;

        private void Awake()
        {
            Debug.Log("Paint mode gizmo created.");

            if (_instance) DestroyVisualizerObject();

            _instance = CreateVisualizerObject();
        }

        private void Update()
        {
            GizmoPosition = CalculateMouseWorldPosition(Input.mousePosition);

            UpdateGizmoPosition();
        }

        public void UpdateEditor()
        {
            UpdateGizmoPosition();
        }

        public void PaintEditor()
        {
            if (OnPaint == null) return;
            
            var gizmoPosition = CalculatePaintPosition(GizmoPosition, Config, BrushSize);
            var gizmoSize = Mathf.Max (1, BrushSize);
            
            var paintPosition = new Vector3();

            for (paintPosition.x = 0; paintPosition.x < gizmoSize; paintPosition.x++)
            {
                for (paintPosition.z = 0; paintPosition.z < gizmoSize; paintPosition.z++)
                {
                    var position = gizmoPosition + paintPosition;

                    OnPaint(position);
                }
            }
        }

        private void OnApplicationQuit()
        {
            DestroyVisualizerObject();
        }
        
        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += ManageStateChange;
        }

        private void OnDestroy()
        {
            DestroyVisualizerObject();
        }
        
        private void ManageStateChange(PlayModeStateChange state)
        {
            DestroyVisualizerObject();
        }

        private GameObject CreateVisualizerObject()
        {
            var gizmoObject = new GameObject {name = "Paint Mode Gizmo"};

            _gizmoMeshRenderer = gizmoObject.AddComponent<MeshRenderer>();
            _gizmoMeshRenderer.material = new Material(Shader.Find("DungeonArchitect/Unlit/Transparent"));

            _gizomoMeshFilter = gizmoObject.AddComponent<MeshFilter>();
            _gizomoMeshFilter.mesh = Resources.Load<Mesh>("Gizmos/PaintGizmo");

            return gizmoObject;
        }

        private void DestroyVisualizerObject()
        {
            Debug.Log("Paint mode gizmo destroyed.");

            DestroyImmediate(_instance);
        }

        private void UpdateGizmoPosition()
        {
            _instance.transform.position = SnapToGrid(GizmoPosition);
            _instance.transform.localScale = Vector3.one * BrushSize;
        }

        private Vector3 SnapToGrid(Vector3 value)
        {
            var gridSize = Config.GridCellSize * BrushSize;
            value.x = Mathf.FloorToInt(value.x / gridSize.x) * gridSize.x;
            value.y = 0 * gridSize.y;
            value.z = Mathf.FloorToInt(value.z / gridSize.z) * gridSize.z;
            return value;
        }

        private static Vector3 CalculatePaintPosition(Vector3 gizmoPosition, PaintModeDungeonConfig config, int brushSize)
        {
            var gridSize = config.GridCellSize * brushSize;
            
            return new Vector3(
                Mathf.RoundToInt(gizmoPosition.x / gridSize.x),
                Mathf.RoundToInt(gizmoPosition.y / gridSize.y),
                Mathf.RoundToInt(gizmoPosition.z / gridSize.z)
            );
        }

        private static Vector3 CalculateMouseWorldPosition(Vector2 mousePosition)
        {
            float distance;

            const int cursorHeight = 0;
            var planePoint = new Vector3(0, cursorHeight, 0);
            var plane = new Plane(Vector3.up, planePoint);
            var hitpoint = new Vector3();

            var ray = Camera.main.ScreenPointToRay(mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                hitpoint = ray.GetPoint(distance);
            }

            return hitpoint;
        }
    }
}