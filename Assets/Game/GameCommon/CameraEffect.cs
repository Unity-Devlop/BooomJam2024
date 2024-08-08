using Cinemachine;
using UnityEngine;

namespace Game
{
    public class CameraEffect : MonoBehaviour
    {
        private CinemachineVirtualCamera _virtualCamera;
        private CinemachineBasicMultiChannelPerlin _noiseProfile;

        void Awake()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _noiseProfile = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        /// <summary>
        /// 震屏
        /// </summary>
        /// <param name="duration">时长</param>
        /// <param name="amplitude">幅度</param>
        /// <param name="frequency">频率</param>
        public void Shake(float duration = 0.25f, float amplitude = 1f, float frequency = 1f)
        {
            // if (_noiseProfile != null)
            // {
            //     _noiseProfile.m_AmplitudeGain = amplitude;
            //     _noiseProfile.m_FrequencyGain = frequency;
            //     Invoke(nameof(StopShaking), duration);
            // }
        }
        //
        // /// <summary>
        // /// 停止震屏
        // /// </summary>
        // private void StopShaking()
        // {
        //     if (_noiseProfile != null)
        //     {
        //         _noiseProfile.m_AmplitudeGain = 0f;
        //         _noiseProfile.m_FrequencyGain = 0f;
        //     }
        // }
    }
}