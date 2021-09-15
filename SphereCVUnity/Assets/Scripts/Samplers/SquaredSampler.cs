using System;
using Unity.Mathematics;
using UnityEngine.Perception.Randomization.Samplers;

namespace Runtime.Samplers
{
    [Serializable]
    public class SquaredSampler : ISampler
    {
        public FloatRange range = new FloatRange(0f, 1f);

        public SquaredSampler() {}
        
        public SquaredSampler(float minimum, float maximum)
        {
            range = new FloatRange(minimum, maximum);
        }
        
        public float Sample()
        {
            var rng = SamplerState.CreateGenerator();
            var randomValue = rng.NextFloat();
            randomValue *= randomValue;
            return math.lerp(range.minimum, range.maximum, randomValue);
        }

        public void Validate()
        {
            range.Validate();
        }
    }
}
