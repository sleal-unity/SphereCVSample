// using System;
// using UnityEngine;
// using UnityEngine.Perception.Randomization.Randomizers;
// using UnityEngine.Perception.Randomization.Samplers;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.HighDefinition;
// using FloatParameter = UnityEngine.Perception.Randomization.Parameters.FloatParameter;
//
// namespace RedBall.Randomizers
// {
//     [Serializable]
//     [AddRandomizerMenu("RedBall/Exposure Randomizer")]
//     public class ExposureRandomizer : Randomizer
//     {
//         public Volume volume;
//         public FloatParameter exposureRange = new FloatParameter { value = new NormalSampler
//         {
//             range = new FloatRange(7f, 9.4f),
//             mean = 7.8f,
//             standardDeviation = 0.5f
//         }};
//             
//         Exposure m_ExposureComponent;
//
//         protected override void OnScenarioStart()
//         {
//             volume.sharedProfile.TryGet(out m_ExposureComponent);
//         }
//
//         protected override void OnIterationStart()
//         {
//             m_ExposureComponent.fixedExposure = new UnityEngine.Rendering.FloatParameter(exposureRange.Sample(), true);
//         }
//     }
// }
