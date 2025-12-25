using UnityEngine;
using UnityEngine.Audio;

namespace DreemurrStudio.AudioSystem
{
    /// <summary>
    /// 播放音源相关的动画事件
    /// </summary>
    public class AnimationEvent_Audio : MonoBehaviour
    {
        /// <summary>
        /// 播放世界中音效
        /// </summary>
        /// <param name="seName">要播放的音效名</param>
        public void PlayWorldSFX(string seName) => 
            SFXManager.Instance.PlayWorldSFX(seName, gameObject);
        
        /// <summary>
        /// 播放全局音效
        /// </summary>
        /// <param name="seName">要播放的音效名</param>
        public void PlayOverlaySFX(string seName) => 
            SFXManager.Instance.PlayOverlaySFX(seName);

        /// <summary>
        /// 切换到目标快照状态
        /// </summary>
        /// <param name="aimShot">目标快照</param>
        public void SwitchToSnapshot(AudioMixerSnapshot aimShot) =>
            AudioManager.Instance.SwitchToSnapshot(aimShot, 0f);
    }
}
