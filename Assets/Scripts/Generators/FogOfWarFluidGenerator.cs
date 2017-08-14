using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Generators
{
    public class FogOfWarFluidGenerator : MonoBehaviour
    {
        private bool _simulating = true;

        private int _width = 512;

        public int Width
        {
            get { return _width; }
        }

        private int _height = 512;

        public int Height
        {
            get { return _height; }
        }

        private float _scale = 1;

        public float Scale
        {
            get { return _scale; }
        }

        private int _iterations = 40;
        private float _deltaTime = 0.125f;
        private float _velocityViscosity = 0.001f;
        private float _densityViscosity = 0.001f;
        private float _ambientTemperature = 0.0f;
        private float _smokeBuoyancy = 5.0f;
        private float _smokeWeight = 0.05f;
        private float _fadeSpeed = 0.99f;

        private RenderTexture _divergenceTex;
        private RenderTexture _obstaclesTex;
        private RenderTexture[] _velocityTex;
        private RenderTexture[] _colorTex;
        private RenderTexture[] _pressureTex;
        private RenderTexture[] _temperatureTex;

        private Material _advectMat;

        private Material _obstaclesMat;
        private Material _divergenceMat;
        private Material _jacobi1DMat;
        private Material _jacobi2DMat;
        private Material _jacobi3DMat;
        private Material _gradientMat;
        private Material _impluseMat;
        private Material _buoyancyMat;
        private Material _fadeMat;

        private Vector2 _inverseSize;

        public void SetSimulating(bool simulating)
        {
            _simulating = simulating;
        }

        public void SetViscosity(float viscosity)
        {
            _velocityViscosity = viscosity;
            _densityViscosity = viscosity;
        }

        public void SetBuoyancy(float buoyancy)
        {
            _smokeBuoyancy = buoyancy;
        }

        public void SetFadeSpeed(float fadeSpeed)
        {
            _fadeSpeed = fadeSpeed;
        }

        public void Setup(int width, int height, float scale, float deltaT)
        {
            _width = width;
            _height = height;
            _scale = scale;
            _deltaTime = deltaT;

            // Compute helpter vars
            _inverseSize = new Vector2(1.0f / _width, 1.0f / _height) / scale;

            CreateAllTextures();
            CreateAllMaterials();

            AddObstacles();
        }

        private void CreateTexture(ref RenderTexture texture, RenderTextureFormat format, FilterMode filter)
        {
            texture = new RenderTexture(_width, _height, 0, format)
            {
                filterMode = filter,
                wrapMode = TextureWrapMode.Clamp
            };
            texture.Create();

            Graphics.SetRenderTarget(texture);
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            Graphics.SetRenderTarget(null);
        }

        private void CreateAllTextures()
        {
            CreateTexture(ref _divergenceTex, RenderTextureFormat.RFloat, FilterMode.Point);

            CreateTexture(ref _obstaclesTex, RenderTextureFormat.ARGB32, FilterMode.Bilinear);

            _velocityTex = new RenderTexture[2];
            CreateTexture(ref _velocityTex[0], RenderTextureFormat.RGFloat, FilterMode.Bilinear);
            CreateTexture(ref _velocityTex[1], RenderTextureFormat.RGFloat, FilterMode.Bilinear);

            _colorTex = new RenderTexture[2];
            CreateTexture(ref _colorTex[0], RenderTextureFormat.ARGB32, FilterMode.Bilinear);
            CreateTexture(ref _colorTex[1], RenderTextureFormat.ARGB32, FilterMode.Bilinear);

            _pressureTex = new RenderTexture[2];
            CreateTexture(ref _pressureTex[0], RenderTextureFormat.RFloat, FilterMode.Point);
            CreateTexture(ref _pressureTex[1], RenderTextureFormat.RFloat, FilterMode.Point);

            _temperatureTex = new RenderTexture[2];
            CreateTexture(ref _temperatureTex[0], RenderTextureFormat.RFloat, FilterMode.Bilinear);
            CreateTexture(ref _temperatureTex[1], RenderTextureFormat.RFloat, FilterMode.Bilinear);
        }

        private void _ReleaseAllTextures()
        {
            _divergenceTex.Release();
            _obstaclesTex.Release();
            _velocityTex[0].Release();
            _velocityTex[1].Release();
            _colorTex[0].Release();
            _colorTex[1].Release();
            _pressureTex[0].Release();
            _pressureTex[1].Release();
            _temperatureTex[0].Release();
            _temperatureTex[1].Release();
        }

        private void CreateAllMaterials()
        {
            _advectMat = new Material(Shader.Find("FogOfWar/Simulation/Advect"));
            _obstaclesMat = new Material(Shader.Find("FogOfWar/Simulation/Obstacle"));
            _divergenceMat = new Material(Shader.Find("FogOfWar/Simulation/Divergence"));
            _jacobi1DMat = new Material(Shader.Find("FogOfWar/Simulation/Jacobi1d"));
            _jacobi2DMat = new Material(Shader.Find("FogOfWar/Simulation/Jacobi2d"));
            _jacobi3DMat = new Material(Shader.Find("FogOfWar/Simulation/Jacobi3d"));
            _gradientMat = new Material(Shader.Find("FogOfWar/Simulation/SubtractGradient"));
            _impluseMat = new Material(Shader.Find("FogOfWar/Simulation/Impulse"));
            _buoyancyMat = new Material(Shader.Find("FogOfWar/Simulation/Buoyancy"));
            _fadeMat = new Material(Shader.Find("FogOfWar/Simulation/Fade"));
        }

        private void AddObstacles()
        {
            _obstaclesMat.SetVector("_InverseSize", _inverseSize);
            _obstaclesMat.SetFloat("_Scale", _scale);

            Graphics.Blit(null, _obstaclesTex, _obstaclesMat);
        }

        public RenderTexture GetFuildDrawInfo()
        {
            return _colorTex[0];
        }

        public RenderTexture GetObstacleTexture()
        {
            return _obstaclesTex;
        }

        public void AddSource(Vector2 pos, Vector3 amount, float radius)
        {
            ApplyImpulse(_colorTex[0], pos, amount, radius);

            var tempAmount = (amount.x * 0.299f + amount.y * 0.587f + amount.z * 0.114f) * 10.0f;
            ApplyImpulse(_temperatureTex[0], pos, new Vector3(tempAmount, tempAmount, tempAmount), radius);
        }

        private static void Swap(IList<RenderTexture> textures)
        {
            var temp = textures[0];
            textures[0] = textures[1];
            textures[1] = temp;
        }

        private static void ClearTexture(RenderTexture texture)
        {
            Graphics.SetRenderTarget(texture);
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            Graphics.SetRenderTarget(null);
        }

        private void Advect(Texture source, RenderTexture destination, Texture verlocity)
        {
            _advectMat.SetTexture("_SrcTex", source);
            _advectMat.SetTexture("_VelocityTex", verlocity);
            _advectMat.SetTexture("_ObstacleTex", _obstaclesTex);
            _advectMat.SetFloat("_DeltaT", _deltaTime);

            Graphics.Blit(null, destination, _advectMat);
        }

        private void ComputeDivergence(Texture verlocity, RenderTexture dest)
        {
            _divergenceMat.SetTexture("_VelocityTex", verlocity);
            _divergenceMat.SetTexture("_ObstacleTex", _obstaclesTex);

            Graphics.Blit(null, dest, _divergenceMat);
        }

        private void Jacobi1d(Texture xTex, Texture bTex, RenderTexture dest, float alpha, float beta)
        {
            _jacobi1DMat.SetTexture("_XTex", xTex);
            _jacobi1DMat.SetTexture("_BTex", bTex);
            _jacobi1DMat.SetFloat("_Alpha", alpha);
            _jacobi1DMat.SetFloat("_rBeta", 1.0f / beta);
            _jacobi1DMat.SetTexture("_ObstacleTex", _obstaclesTex);

            Graphics.Blit(null, dest, _jacobi1DMat);
        }

        private void Jacobi2d(Texture xTex, Texture bTex, RenderTexture dest, float alpha, float beta)
        {
            _jacobi2DMat.SetTexture("_XTex", xTex);
            _jacobi2DMat.SetTexture("_BTex", bTex);
            _jacobi2DMat.SetFloat("_Alpha", alpha);
            _jacobi2DMat.SetFloat("_rBeta", 1.0f / beta);
            _jacobi2DMat.SetTexture("_ObstacleTex", _obstaclesTex);

            Graphics.Blit(null, dest, _jacobi2DMat);
        }

        private void Jacobi3d(Texture xTex, Texture bTex, RenderTexture dest, float alpha, float beta)
        {
            _jacobi3DMat.SetTexture("_XTex", xTex);
            _jacobi3DMat.SetTexture("_BTex", bTex);
            _jacobi3DMat.SetFloat("_Alpha", alpha);
            _jacobi3DMat.SetFloat("_rBeta", 1.0f / beta);
            _jacobi3DMat.SetTexture("_ObstacleTex", _obstaclesTex);

            Graphics.Blit(null, dest, _jacobi3DMat);
        }

        private void SubtractGradient(Texture velocity, Texture pressure, RenderTexture dest)
        {
            _gradientMat.SetTexture("_VelocityTex", velocity);
            _gradientMat.SetTexture("_PressureTex", pressure);
            _gradientMat.SetTexture("_ObstacleTex", _obstaclesTex);

            Graphics.Blit(null, dest, _gradientMat);
        }

        private void Project()
        {
            //Calculates how divergent the velocity is
            ComputeDivergence(_velocityTex[0], _divergenceTex);

            ClearTexture(_pressureTex[0]);

            // Compute pressure
            for (var i = 0; i < _iterations; i++)
            {
                Jacobi1d(_pressureTex[0], _divergenceTex, _pressureTex[1], -1.0f, 4.0f);
                Swap(_pressureTex);
            }

            //Use the pressure tex that was last rendered into. This computes divergence free velocity
            SubtractGradient(_velocityTex[0], _pressureTex[0], _velocityTex[1]);
            Swap(_velocityTex);
        }

        private void VelocityDiffusion()
        {
            for (var i = 0; i < _iterations; ++i)
            {
                var alpha = 1.0f / (_deltaTime * _velocityViscosity);
                Jacobi2d(_velocityTex[0], _velocityTex[0], _velocityTex[1], alpha, 4.0f + alpha);
                Swap(_velocityTex);
            }
        }

        private void DensityDiffusion()
        {
            for (var i = 0; i < _iterations; ++i)
            {
                var alpha = 1.0f / (_deltaTime * _densityViscosity);
                Jacobi3d(_colorTex[0], _colorTex[0], _colorTex[1], alpha, 4.0f + alpha);
                Swap(_colorTex);
            }
        }

        private void ApplyImpulse(RenderTexture dest, Vector2 pos, Vector3 amount, float radius)
        {
            _impluseMat.SetFloat("_Aspect", _inverseSize.y / _inverseSize.x);
            _impluseMat.SetVector("_ImpulsePos", pos);
            _impluseMat.SetVector("_Amount", amount);
            _impluseMat.SetFloat("_Radius", radius / _scale);

            Graphics.Blit(null, dest, _impluseMat);
        }

        private void ApplyBuoyancy(Texture velocity, Texture temperature, Texture density, RenderTexture dest)
        {
            _buoyancyMat.SetTexture("_VelocityTex", velocity);
            _buoyancyMat.SetTexture("_TemperatureTex", temperature);
            _buoyancyMat.SetTexture("_DensityTex", density);
            _buoyancyMat.SetFloat("_AmbientTemperature", _ambientTemperature);
            _buoyancyMat.SetFloat("_DeltaT", _deltaTime);
            _buoyancyMat.SetFloat("_Sigma", _smokeBuoyancy);
            _buoyancyMat.SetFloat("_Kappa", _smokeWeight);

            Graphics.Blit(null, dest, _buoyancyMat);
        }

        private void Fade(Texture src, RenderTexture dest, float speed)
        {
            _fadeMat.SetTexture("_SrcTex", src);
            _fadeMat.SetFloat("_FadeSpeed", speed);

            Graphics.Blit(null, dest, _fadeMat);
        }

        private void Update()
        {
            if (!_simulating) return;

            AddObstacles();

            // 1. Self advect
            Advect(_velocityTex[0], _velocityTex[1], _velocityTex[0]);
            Swap(_velocityTex);

            // 2. Pressure
            Project();

            // 3. Diffusion (viscosity)
            VelocityDiffusion();

            Advect(_colorTex[0], _colorTex[1], _velocityTex[0]);
            Swap(_colorTex);

            // 2. Diffusion
            DensityDiffusion();

            Advect(_temperatureTex[0], _temperatureTex[1], _velocityTex[0]);
            Swap(_temperatureTex);

            ApplyBuoyancy(_velocityTex[0], _temperatureTex[0], _colorTex[0], _velocityTex[1]);
            Swap(_velocityTex);

            Fade(_colorTex[0], _colorTex[1], _fadeSpeed);
            Swap(_colorTex);
        }

        private void OnDestroy()
        {
            _ReleaseAllTextures();
        }
    }
}