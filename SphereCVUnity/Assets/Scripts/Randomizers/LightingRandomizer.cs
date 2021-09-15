using System;
using Runtime.Samplers;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine.Rendering.HighDefinition;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Lighting Randomizer")]
    public class LightingRandomizer : Randomizer
    {
        public Light ceilingPointLight;
        public float radius = 5.75f;
        public FloatParameter intensity = new FloatParameter { value = new ExponentialSampler(1f, 7f)};

        Vector2Parameter m_Rotation = new Vector2Parameter
        {
            x = new UniformSampler(-180f, 180f),
            y = new UniformSampler(25f, 90f)
        };
        HDAdditionalLightData m_LightData;

        protected override void OnScenarioStart()
        {
            m_LightData = ceilingPointLight.GetComponent<HDAdditionalLightData>();
        }

        protected override void OnIterationStart()
        {
            m_LightData.intensity = intensity.Sample();
            var angle = m_Rotation.Sample();
            var rotation = Quaternion.Euler(angle.x, angle.y, 0f);
            var transform = ceilingPointLight.transform;
            transform.position = rotation * Vector3.forward * radius;
        }
    }
}
