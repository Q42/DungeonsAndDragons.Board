using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(ParticleSystem))]
    public class FogOfWarController : MonoBehaviour
    {
        private MeshFilter _particleMeshFilter;
        private ParticleSystem _particleSystem;

        private int _particleCountInitial;

        private void Awake()
        {
            _particleMeshFilter = GetComponent<MeshFilter>();
            _particleSystem = GetComponent<ParticleSystem>();

            _particleCountInitial = _particleSystem.main.maxParticles;
        }

        public void Simulate()
        {
            _particleSystem.Simulate(100f);
            _particleSystem.Play();
        }
        
        public void SetParticleMesh(Mesh mesh, int size)
        {
            var particleSystemMain = _particleSystem.main;
            var particleSystemEmission = _particleSystem.emission;

            _particleMeshFilter.mesh = mesh;
            particleSystemMain.maxParticles = _particleCountInitial * size;
            particleSystemEmission.rateOverTime = _particleCountInitial * size;
        }
    }
}