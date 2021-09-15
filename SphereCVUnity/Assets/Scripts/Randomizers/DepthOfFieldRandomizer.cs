using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using FloatParameter = UnityEngine.Perception.Randomization.Parameters.FloatParameter;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Depth of Field Randomizer")]
    public class DepthOfFieldRandomizer : Randomizer
    {
        public Volume volume;
        public FloatParameter nearFocus = new FloatParameter { value = new NormalSampler(0.2f, 0.4f, 0.3f, 0.1f)};
        public FloatParameter farFocus = new FloatParameter { value = new NormalSampler(0.75f, 2.75f, 1.75f, 0.4f)};

        DepthOfField m_DepthOfFieldComponent;

        protected override void OnScenarioStart()
        {
            volume.profile.TryGet(out m_DepthOfFieldComponent);
        }
        
        protected override void OnIterationStart()
        {
            var near = nearFocus.Sample();
            var far = farFocus.Sample();
            m_DepthOfFieldComponent.nearFocusEnd.SetValue(new MinFloatParameter(near, 0f, true));
            m_DepthOfFieldComponent.farFocusStart.SetValue(new MinFloatParameter(far, 0f, true));
        }
    }
}
