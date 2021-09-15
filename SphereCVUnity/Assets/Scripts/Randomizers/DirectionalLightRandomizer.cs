using System;
using UnityEngine;
using UnityEngine.Perception.Randomization.Parameters;
using UnityEngine.Perception.Randomization.Randomizers;
using UnityEngine.Perception.Randomization.Samplers;

namespace RedBall.Randomizers
{
    [Serializable]
    [AddRandomizerMenu("RedBall/Directional Light Randomizer")]
    public class DirectionalLightRandomizer : Randomizer
    {
        public Light directionalLight;
        public FloatParameter intensity = new FloatParameter { value = new UniformSampler(5000f, 80000f)};
        public Vector2Parameter rotation;
        
        protected override void OnIterationStart()
        {
            directionalLight.intensity = intensity.Sample();
            var lightRotation = rotation.Sample();
            directionalLight.transform.rotation = Quaternion.Euler(lightRotation.x, lightRotation.y, 0f);
        }
    }
}
