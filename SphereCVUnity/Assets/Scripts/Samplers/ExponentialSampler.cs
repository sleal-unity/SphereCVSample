using System;
using Unity.Mathematics;
using UnityEngine.Perception.Randomization.Samplers;

namespace Runtime.Samplers
{
    public class ExponentialSampler : ISampler
    {
        public FloatRange range = new FloatRange(0f, 1f);

        public ExponentialSampler() {}
        
        public ExponentialSampler(float minimum, float maximum)
        {
            range = new FloatRange(minimum, maximum);
        }
        
        public float Sample()
        {
            var x = math.log10(range.minimum);
            var y = math.log10(range.maximum);
            var rng = SamplerState.CreateGenerator();
            var randomValue = rng.NextFloat();
            return math.pow(10, math.lerp(x, y, randomValue));
        }

        public void Validate()
        {
            range.Validate();
        }
    }
}
