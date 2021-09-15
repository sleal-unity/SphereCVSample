using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Room Shell Randomizer")]
    public class RoomShellRandomizer : Randomizer
    {
        static readonly int k_HueShift = Shader.PropertyToID("_HueShift");
        
        public GameObject roomShell;
        public FloatParameter positionOffset = new FloatParameter { value = new UniformSampler(-1f, 1f) };
        public FloatParameter hueOffset = new FloatParameter { value = new NormalSampler(-180f, 180f, 0f, 7.5f) };

        Vector3 m_InitialPosition;
        List<Material> m_RoomMaterials = new List<Material>();

        protected override void OnScenarioStart()
        {
            m_InitialPosition = roomShell.transform.position;
            var renderers = roomShell.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
                m_RoomMaterials.Add(renderer.material);
        }

        protected override void OnIterationStart()
        {
            roomShell.transform.position = m_InitialPosition + Vector3.right * positionOffset.Sample();
            foreach (var material in m_RoomMaterials)
                material.SetFloat(k_HueShift, hueOffset.Sample());
        }
    }
}
