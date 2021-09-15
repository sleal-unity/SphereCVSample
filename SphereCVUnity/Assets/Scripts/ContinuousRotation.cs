using UnityEngine;

namespace Runtime
{
    [AddComponentMenu("Test/Continuous Rotation")]
    public class ContinuousRotation : MonoBehaviour
    {
        const float k_Speed = 0.1f * (180f / 256f);
        
        void Update()
        {
            transform.rotation *= Quaternion.Euler(0f, 0f, k_Speed);
        }
    }
}
