using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Scenarios;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Accumulation Motion Blur Manager")]
    public class AccumulationMotionBlurManager : Randomizer
    {
        /// <summary>
        /// The portion of the generated from in which the shutter is open?
        /// </summary>
        [Range(0.0f, 1.0f)]
        public float shutterInterval = 1.0f;

        // The time during shutter interval when the shutter is fully open
        [Range(0.0f, 1.0f)]
        public float shutterFullyOpen = 0.25f;

        // The time during shutter interval when the shutter begins closing.
        [Range(0.0f, 1.0f)]
        public float shutterBeginsClosing = 0.75f;

        FixedLengthScenario.Constants m_Constants;

        HDRenderPipeline renderPipeline => (HDRenderPipeline)RenderPipelineManager.currentPipeline;
        
        protected override void OnScenarioStart()
        {
            m_Constants = (FixedLengthScenario.Constants)scenario.genericConstants;
        }

        protected override void OnIterationStart()
        {
            BeginMultiFrameRendering();
        }

        protected override void OnIterationEnd()
        {
            StopMultiFrameRendering();
        }

        void BeginMultiFrameRendering()
        {
            // RenderPipelineManager.beginFrameRendering += PrepareSubFrameCallBack;
            renderPipeline.BeginRecording(
                m_Constants.framesPerIteration - 1, shutterInterval, shutterFullyOpen, shutterBeginsClosing);
        }
        
        void StopMultiFrameRendering()
        {
            // RenderPipelineManager.beginFrameRendering -= PrepareSubFrameCallBack;
            renderPipeline.EndRecording();
        }

        void PrepareSubFrameCallBack(ScriptableRenderContext cntx, Camera[] cams)
        {
            renderPipeline.PrepareNewSubFrame();
        }
    }
}
