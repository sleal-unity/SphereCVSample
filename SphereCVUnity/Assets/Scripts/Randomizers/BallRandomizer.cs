using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Ball Randomizer")]
    public class BallRandomizer : Randomizer
    {
        static readonly int k_HueShift = Shader.PropertyToID("_HueShift");
        
        public GameObject ball;
        public Vector3Parameter positionOffset = new Vector3Parameter
        {
            x = new UniformSampler(-0.2f, 0.2f),
            y = new UniformSampler(-0.2f, 0.2f),
            z = new ConstantSampler(0f)
        };
        public FloatParameter scaleFactor = new FloatParameter { value = new UniformSampler(0.2f, 2f)};
        public FloatParameter hueShift = new FloatParameter { value = new NormalSampler(-180f, 180f, 0f, 5f)};

        Material m_BallMaterial;
        Vector3 m_InitialPosition;
        Vector3 m_InitialScale;

        protected override void OnScenarioStart()
        {
            m_InitialPosition = ball.transform.position;
            m_InitialScale = ball.transform.localScale;
            m_BallMaterial = ball.GetComponent<Renderer>().material;
        }

        protected override void OnIterationStart()
        {
            ball.transform.position = m_InitialPosition + positionOffset.Sample();
            ball.transform.localScale = m_InitialScale * scaleFactor.Sample();
            m_BallMaterial.SetFloat(k_HueShift, hueShift.Sample());
        }
    }
}

