using System;
using RedBall.Scenarios;
using Runtime.Samplers;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Ball Placement Randomizer")]
    public class BallPlacementRandomizer : Randomizer
    {
        static readonly int k_HueShift = Shader.PropertyToID("_HueShift");
        static readonly int k_DesaturationShift = Shader.PropertyToID("_DesaturationShift");
        
        public GameObject ball;
        public FloatParameter distanceFromCamera = new FloatParameter { value = new SquaredSampler(0.05f, 1f)};
        public BooleanParameter moveBallDuringIteration = new BooleanParameter { threshold = 0.8f };
        public FloatParameter viewPortDistanceToMove = new FloatParameter { value = new UniformSampler(0.01f, 0.15f) };
        
        public FloatParameter hueOffset = new FloatParameter{ value = new NormalSampler(-180f, 180f, 0f, 5f)};
        public FloatParameter desaturationOffset = new FloatParameter{ value = new NormalSampler(0f, 0.3f, 0f, 0.15f)};

        Camera m_Camera;
        UniformSampler m_Sampler = new UniformSampler(0f, 1f);
        UniformSampler m_AngleSampler = new UniformSampler(-Mathf.PI, Mathf.PI);
        
        Vector3 m_StartPosition;
        Vector3 m_EndPosition;

        BallScenario ballScenario => (BallScenario)scenario;

        Material m_BallMaterial;

        protected override void OnScenarioStart()
        {
            m_Camera = Camera.main;
            m_BallMaterial = ball.GetComponent<MeshRenderer>().sharedMaterial;
        }

        protected override void OnIterationStart()
        {
            m_BallMaterial.SetFloat(k_HueShift, hueOffset.Sample());
            m_BallMaterial.SetFloat(k_DesaturationShift, desaturationOffset.Sample());
            
            var randomViewPortPoint = new Vector3(m_Sampler.Sample(), m_Sampler.Sample(), distanceFromCamera.Sample());
            m_StartPosition = m_Camera.ViewportToWorldPoint(randomViewPortPoint);

            if (moveBallDuringIteration.Sample())
            {
                Vector3 viewPortEndPoint;
                do
                {
                    var randomAngle = m_AngleSampler.Sample();
                    var viewPortTravelDist = viewPortDistanceToMove.Sample();
                    var viewPortOffset = 
                        new Vector3(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle), 0f) * viewPortTravelDist;
                    viewPortEndPoint = randomViewPortPoint + viewPortOffset;
                } while (viewPortEndPoint.x < 0f || viewPortEndPoint.x > 1f ||
                    viewPortEndPoint.y < 0f || viewPortEndPoint.y > 1f);
                m_EndPosition = m_Camera.ViewportToWorldPoint(viewPortEndPoint);
            }
            else
            {
                m_EndPosition = m_StartPosition;
            }
        }

        protected override void OnUpdate()
        {
            var t = (float)ballScenario.currentIterationFrame / ballScenario.constants.framesPerIteration;
            ball.transform.position = Vector3.Lerp(m_StartPosition, m_EndPosition, t);
        }
    }
}
