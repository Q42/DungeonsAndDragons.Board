using System.Collections.Generic;
using System.Linq;
using Generators;
using UnityEngine;

namespace Controllers
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
    public class FogOfWarController : MonoBehaviour
    {
        public int Width = 512;
        public int Height = 512;

        [SerializeField, SetProperty("simulating")]
        private bool _simulating = true;

        public bool Simulating
        {
            get { return _simulating; }
            set
            {
                _simulating = value;
                if (_fogOfWarFluidGenerator != null)
                {
                    _fogOfWarFluidGenerator.SetSimulating(_simulating);
                }
            }
        }

        [SerializeField, SetProperty("transparency")]
        private float _transparency = 5.0f;

        public float Transparency
        {
            get { return _transparency; }
            set
            {
                _transparency = Mathf.Clamp(value, 1.0f, 15.0f);
                if (_fogOfWarGenerator != null)
                {
                    _fogOfWarGenerator.SetTransparancy(_transparency);
                }
            }
        }

        [SerializeField, SetProperty("viscosity")]
        private float _viscosity = 0.001f;

        public float Viscosity
        {
            get { return _viscosity; }
            set
            {
                _viscosity = Mathf.Clamp(value, 0.00000001f, 0.01f);
                if (_fogOfWarFluidGenerator != null)
                {
                    _fogOfWarFluidGenerator.SetViscosity(_viscosity);
                }
            }
        }

        [SerializeField, SetProperty("smokeBuoyancy")]
        private float _smokeBuoyancy = 5.0f;

        public float SmokeBuoyancy
        {
            get { return _smokeBuoyancy; }
            set
            {
                _smokeBuoyancy = Mathf.Clamp(value, 3.0f, 200.0f);
                if (_fogOfWarFluidGenerator != null)
                {
                    _fogOfWarFluidGenerator.SetBuoyancy(_smokeBuoyancy);
                }
            }
        }

        [SerializeField, SetProperty("fadeSpeed")]
        private float _fadeSpeed = 0.99f;

        public float FadeSpeed
        {
            get { return _fadeSpeed; }
            set
            {
                _fadeSpeed = Mathf.Clamp(value, 0.95f, 1.0f);
                if (_fogOfWarFluidGenerator != null)
                {
                    _fogOfWarFluidGenerator.SetFadeSpeed(_fadeSpeed);
                }
            }
        }

        [SerializeField, SetProperty("sourceDensity")]
        private float _sourceDensity = 0.1f;

        public float SourceDensity
        {
            get { return _sourceDensity; }
            set { _sourceDensity = Mathf.Clamp(value, 0.02f, 0.2f); }
        }

        private FogOfWarFluidGenerator _fogOfWarFluidGenerator;
        private FogOfWarGenerator _fogOfWarGenerator;

        private Vector3 _fluidColor;
        private List<float> _fluidScale;
        
        private MeshFilter _meshFilter;
        private MeshCollider _meshColider;

        private void Awake()
        {
            var sizeX = 20;
            var sizeY = 20;
            
            _fluidScale = new List<float> {
                sizeX / 10f,
                sizeY / 10f
            };
            
            _meshFilter = GetComponent<MeshFilter>();
            _meshColider = GetComponent<MeshCollider>();
            
            _meshColider.sharedMesh = _meshFilter.mesh = GridMeshGenerator.GenerateGrid(sizeX, sizeY);
        }

        public void SetFogMesh(Mesh mesh)
        {
            _meshFilter.mesh = mesh;
        }

        void Start()
        {
            _fogOfWarFluidGenerator = gameObject.AddComponent<FogOfWarFluidGenerator>();
            _fogOfWarFluidGenerator.Setup(Width, Height, _fluidScale.Max(), 0.03f);
            _fogOfWarFluidGenerator.SetViscosity(_viscosity);
            _fogOfWarFluidGenerator.SetBuoyancy(_smokeBuoyancy);
            _fogOfWarFluidGenerator.SetFadeSpeed(_fadeSpeed);

            _fogOfWarGenerator = gameObject.AddComponent<FogOfWarGenerator>();
            _fogOfWarGenerator.Setup(_fogOfWarFluidGenerator);
            _fogOfWarGenerator.SetTransparancy(_transparency);
        }

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKeyDown("left"))
            {
                SmokeBuoyancy = SmokeBuoyancy - 1.0f;
            }
            else if (Input.GetKeyDown("right"))
            {
                SmokeBuoyancy = SmokeBuoyancy + 1.0f;
            }
            else if (Input.GetKeyDown("up"))
            {
                SourceDensity = SourceDensity + 0.05f;
            }
            else if (Input.GetKeyDown("down"))
            {
                SourceDensity = SourceDensity - 0.05f;
            }
        }

        private void AddSource()
        {
            RaycastHit hit;
            var cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
            var cursorRay = Camera.main.ScreenPointToRay(cursorPos);
            if (!Physics.Raycast(cursorRay, out hit, 200)) return;

            var meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null) return;

            var pixelUv = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            _fogOfWarFluidGenerator.AddSource(pixelUv, _fluidColor, _sourceDensity);
        }

        private void OnMouseDrag()
        {
            AddSource();
        }

        private void OnMouseDown()
        {
            _fluidColor = new Vector3(1, 0, 0);
            AddSource();
        }

        private void OnMouseExit()
        {
            AddSource();
        }
    }
}