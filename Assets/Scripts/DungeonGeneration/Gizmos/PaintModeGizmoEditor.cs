using UnityEditor;
using UnityEngine;

namespace DungeonGeneration.Gizmos
{
    [CustomEditor(typeof(PaintModeGizmo))]
    public class PaintModeGizmoEditor : Editor
    {
        private SerializedProperty _brushSize;

        private PaintModeGizmo _gizmo;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.IntSlider(_brushSize, 1, 100, new GUIContent("Brush Size"));
            ProgressBar(_brushSize.intValue / 100.0f, "Brush Size");

            serializedObject.ApplyModifiedProperties();
        }

        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate += SceneGUI;
            _gizmo = target as PaintModeGizmo;

            _brushSize = serializedObject.FindProperty("BrushSize");
        }

        private void SceneGUI(SceneView sceneview)
        {
            if (_gizmo == null) return;

            _gizmo.GizmoPosition = CalculateMouseWorldPosition();
            _gizmo.UpdateEditor();

            HandlePaintEvents();
        }

        private void HandlePaintEvents()
        {
            var current = Event.current;
            
            if (current.alt) return;
            
            if (current.button != 0) return;

            switch (current.type)
            {
                case EventType.MouseDown:
                    _gizmo.PaintEditor();
                    current.Use();
                    break;
                case EventType.MouseDrag:
                    _gizmo.PaintEditor();
                    current.Use();
                    break;
                case EventType.Layout:
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
                    break;
            }
        }

        private static Vector3 CalculateMouseWorldPosition()
        {
            float distance;
            var e = Event.current;

            const int cursorHeight = 0;
            var planePoint = new Vector3(0, cursorHeight, 0);
            var plane = new Plane(Vector3.up, planePoint);
            var hitpoint = new Vector3();

            var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                hitpoint = ray.GetPoint(distance);
            }

            return hitpoint;
        }

        private static void ProgressBar(float value, string label)
        {
            var rect = GUILayoutUtility.GetRect(18, 18, "TextField");
            EditorGUI.ProgressBar(rect, value, label);
            EditorGUILayout.Space();
        }
    }
}