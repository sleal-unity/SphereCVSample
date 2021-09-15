using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Ball Distance DOF Randomizer")]
    public class BallDistanceDofRandomizer : Randomizer
    {
        public GameObject ball;
        public Volume volume;
        public float wallDistance = 6f;

        float m_BallRadius;
        NormalSampler m_BallFocusDistanceOffset;
        NormalSampler m_WallFocusDistanceOffset;
        DepthOfField m_DepthOfFieldComponent;

        protected override void OnScenarioStart()
        {
            m_BallRadius = ball.GetComponent<MeshRenderer>().bounds.extents.x;
            var diameter = m_BallRadius;
            m_BallFocusDistanceOffset = new NormalSampler(-diameter, diameter, 0f, m_BallRadius);
            m_WallFocusDistanceOffset = new NormalSampler(-0.25f, 0.25f, 0f, 0.125f);
            volume.profile.TryGet(out m_DepthOfFieldComponent);
        }
        
        protected override void OnIterationStart()
        {
            var ballDistance = ball.transform.position.magnitude;
            var ballFocusDistance = ballDistance + m_BallFocusDistanceOffset.Sample();
            var wallFocusDistance = wallDistance + m_WallFocusDistanceOffset.Sample();
            
            var nearStart = Mathf.Max(ballFocusDistance - 1f, 0f);
            m_DepthOfFieldComponent.nearFocusStart.SetValue(
                new MinFloatParameter(nearStart, 0f, true));
            
            var nearEnd = Mathf.Max(ballFocusDistance, 0f);
            m_DepthOfFieldComponent.nearFocusEnd.SetValue(
                new MinFloatParameter(nearEnd, 0f, true));
            
            m_DepthOfFieldComponent.farFocusStart.SetValue(
                new MinFloatParameter(wallFocusDistance, 0f, true));
            
            m_DepthOfFieldComponent.farFocusEnd.SetValue(
                new MinFloatParameter(wallFocusDistance + 2f, 0f, true));
        }
    }
}
