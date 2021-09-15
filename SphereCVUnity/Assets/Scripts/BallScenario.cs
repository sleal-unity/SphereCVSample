using UnityEngine;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.Randomization.Scenarios;
using UnityEngine.Rendering;

namespace RedBall.Scenarios
{
    [AddComponentMenu("RedBall/Ball Scenario")]
    public class BallScenario : FixedLengthScenario
    {
        PerceptionCamera m_PerceptionCamera;

        protected override bool isScenarioReadyToStart => RenderPipelineManager.currentPipeline != null;

        protected override void OnStart()
        {
            m_PerceptionCamera = FindObjectOfType<PerceptionCamera>();
        }

        protected override void OnUpdate()
        {
            if (currentIterationFrame == constants.framesPerIteration - 1)
            {
                m_PerceptionCamera.RequestCapture();
            }
        }
    }  
}

