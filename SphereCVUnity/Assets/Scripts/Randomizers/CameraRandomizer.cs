using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Camera Randomizer")]
    public class CameraRandomizer : Randomizer
    {
        UniformSampler m_RotationSampler = new UniformSampler(-180, 180);
        Camera m_Camera;

        protected override void OnScenarioStart()
        {
            m_Camera = Camera.main;
        }

        protected override void OnIterationStart()
        {
            m_Camera.transform.rotation = quaternion.Euler(0f, 0f, m_RotationSampler.Sample());
        }
    }
}
