using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Cube Room Randomizer")]
    public class CubeRoomRandomizer : Randomizer
    {
        static readonly int k_HueShift = Shader.PropertyToID("_HueShift");
        static readonly int k_DesaturationShift = Shader.PropertyToID("_DesaturationShift");
        static readonly int k_BaseMap = Shader.PropertyToID("_BaseMap");
        
        public GameObject room;
        public FloatParameter hueOffset = new FloatParameter{ value = new NormalSampler(-180f, 180f, 0f, 10f)};
        public FloatParameter desaturationOffset = new FloatParameter{ value = new NormalSampler(0f, 0.3f, 0f, 0.15f)};
        public Texture2DParameter textures;

        Material m_Material;
        
        protected override void OnScenarioStart()
        {
            var renderer = room.GetComponentInChildren<MeshRenderer>();
            m_Material = renderer.sharedMaterial;
        }

        protected override void OnIterationStart()
        {
            m_Material.SetFloat(k_HueShift, hueOffset.Sample());
            m_Material.SetFloat(k_DesaturationShift, desaturationOffset.Sample());
            m_Material.SetTexture(k_BaseMap, textures.Sample());
        }
    }
}
