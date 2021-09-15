using System;
using Runtime.Samplers;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Rendering.HighDefinition;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Ceiling Light Randomizer")]
    public class CeilingLightRandomizer : Randomizer
    {
        public Light light;
        public FloatParameter intensity = new FloatParameter { value = new ExponentialSampler(1f, 7f)};

        HDAdditionalLightData m_LightData;

        protected override void OnScenarioStart()
        {
            m_LightData = light.GetComponent<HDAdditionalLightData>();
        }

        protected override void OnIterationStart()
        {
            m_LightData.intensity = intensity.Sample();
        }
    }
}
