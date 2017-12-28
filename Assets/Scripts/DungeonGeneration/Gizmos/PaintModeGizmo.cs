using UnityEngine;

namespace DungeonGeneration.Gizmos
{
    [ExecuteInEditMode]
    public class PaintModeGizmo : MonoBehaviour
    {
        private GameObject _gizmoObject;

        private MeshRenderer _gizmoMeshRenderer;
        private MeshFilter _gizomoMeshFilter;

        private Vector3 _mousePosition;

        private void Awake()
        {
            Debug.Log("Paint mode gizmo created.");

            _gizmoObject = new GameObject {name = "Paint Mode Gizmo"};

            _gizmoMeshRenderer = _gizmoObject.AddComponent<MeshRenderer>();
            _gizmoMeshRenderer.material = new Material(Shader.Find("DungeonArchitect/Unlit/Transparent"));

            _gizomoMeshFilter = _gizmoObject.AddComponent<MeshFilter>();
            _gizomoMeshFilter.mesh = Resources.Load<Mesh>("Gizmos/PaintGizmo");
        }

        private void OnApplicationQuit()
        {
            Debug.Log("Paint mode gizmo destroyed.");

            DestroyImmediate(_gizmoObject);
        }

        private void OnDestroy()
        {
            Debug.Log("Paint mode gizmo destroyed.");

            DestroyImmediate(_gizmoObject);
        }
    }
}