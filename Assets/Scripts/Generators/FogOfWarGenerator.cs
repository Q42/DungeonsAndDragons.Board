using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Generators
{
	public class FogOfWarGenerator : MonoBehaviour
	{
		private FogOfWarFluidGenerator _fluidGenerator;
		private RenderTexture _texture;
		private Material _material;
		private float _transparancy = 1.0f;

		/// <summary>
		/// When the fog of war component is created, get the selected fog of war material.
		/// </summary>
		private void Awake()
		{
			_material = GetComponent<MeshRenderer>().material;
		}

		/// <summary>
		/// On update get the simulated data from the Fog of War simulator.
		/// </summary>
		private void Update()
		{
			var fluidInfoTex = _fluidGenerator.GetFuildDrawInfo();
			var obstacleTex = _fluidGenerator.GetObstacleTexture();
			
			_material.SetTexture("_MainTex", fluidInfoTex);
			_material.SetTexture("_ObstacleTex", obstacleTex);
			_material.SetFloat("_Transparancy", _transparancy);
		}
		
		/// <summary>
		/// Public setter for transparancy value of the fog of war.
		/// </summary>
		/// <param name="transparancy">Transparenct value between 0.0 and 1.0</param>
		public void SetTransparancy(float transparancy) {
			_transparancy = transparancy;
		}

		/// <summary>
		/// When the fog of war component is destroyed, also destroy the render texture to save performance.
		/// </summary>
		private void OnDestroy()
		{
			_texture.Release();
		}

		/// <summary>
		/// This setup can be externally called to initialize all fog of war textures.
		/// </summary>
		/// <param name="fluidGenerator">The fluid simulator containing all properties</param>
		public void Setup(FogOfWarFluidGenerator fluidGenerator)
		{
			_fluidGenerator = fluidGenerator;

			CreateTexture(ref _texture, RenderTextureFormat.ARGB32, FilterMode.Bilinear);
			SetMaterial(_texture);
		}
		
		/// <summary>
		/// Create a fog of war render texture which will later be filled with the simulated fluid.
		/// </summary>
		/// <param name="texture">Reference to the global render texture</param>
		/// <param name="format">Color format to use for the render texture</param>
		/// <param name="filter">Filtering mode to use for the render texture</param>
		private void CreateTexture(ref RenderTexture texture, RenderTextureFormat format, FilterMode filter) {
			texture = new RenderTexture(_fluidGenerator.Width, _fluidGenerator.Height, 0, format)
			{
				filterMode = filter,
				wrapMode = TextureWrapMode.Clamp
			};
			texture.Create();
			
			Graphics.SetRenderTarget(texture);
			GL.Clear(false, true, new Color(0, 0, 0, 0));		
			Graphics.SetRenderTarget(null);
		}

		/// <summary>
		/// Set the freshly generated fog of war texture as the materials main texture.
		/// </summary>
		/// <param name="texture">The render texture to use as material texture</param>
		private void SetMaterial(Texture texture)
		{
			_material.mainTexture = texture;
		}
	}
}
