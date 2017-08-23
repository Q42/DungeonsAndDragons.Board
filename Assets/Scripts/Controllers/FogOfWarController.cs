using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FogOfWarController : MonoBehaviour
    {
        private MeshFilter _particleMeshFilter;

        private void Awake()
        {
            _particleMeshFilter = GetComponent<MeshFilter>();
        }
    }
}