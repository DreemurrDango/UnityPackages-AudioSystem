using UnityEngine;
using UnityEngine.Audio;

namespace DreemurrStudio.AudioSystem
{
    /// <summary>
    /// 音频播放总控制器：控制混音器的音量、快照状态
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField][Tooltip("主混音器对象")]
        private AudioMixer mainMixer;

        [SerializeField]
        [Tooltip("保活音频源，将播放一个几乎无声的白噪声，以确保音频系统始终活跃，可通过开关其Enable属性设置是否保持激活状态")]
        private AudioSource keepAliveSource;

        /// <summary>
        /// 当前的混音状态快照
        /// </summary>
        private AudioMixerSnapshot currentShot;
        protected override void Awake()
        {
            base.Awake();
            keepAliveSource = GetComponent<AudioSource>();
        }

        /// <summary>
        /// 主音量属性：使用线性值来获取/设置
        /// </summary>
        public float MasterVolume
        {
            get => GetMixerVolume("Master");
            set => SetMixerVolume("Master",value);
        }
        /// <summary>
        /// 背景音乐音量属性：使用线性值来获取/设置
        /// </summary>
        public float BGMVolume
        {
            get => GetMixerVolume("BGM");
            set => SetMixerVolume("BGM",value);
        }
        /// <summary>
        /// 环境音音量属性：使用线性值来获取/设置
        /// </summary>
        public float AmbientVolume
        {
            get => GetMixerVolume("Ambient");
            set => SetMixerVolume("Ambient",value);
        }
        /// <summary>
        /// 音效音量属性：使用线性值来获取/设置
        /// </summary>
        public float SFXVolume
        {
            get => GetMixerVolume("SFX");
            set => SetMixerVolume("SFX",value);
        }
        
        /// <summary>
        /// 获取混音器组音量为0~1的线性音量
        /// </summary>
        /// <param name="groupName">要获取的混音器组名</param>
        /// <returns>线性音量大小，范围为0~1</returns>
        private float GetMixerVolume(string groupName)
        {
            mainMixer.GetFloat(groupName + "Volume", out float v);
            return v > 80f ? Mathf.Pow(10f, v / 20f) : 0;
        }
        /// <summary>
        /// 使用线性值设置混音器组音量
        /// </summary>
        /// <param name="groupName">混音器组名</param>
        /// <param name="value">要设置的线性值，一般为0~1</param>
        private void SetMixerVolume(string groupName,float value) =>
            mainMixer.SetFloat(groupName + "Volume", value > 0 ? 20f * Mathf.Log10(value) : -80f);

        /// <summary>
        /// 切换到目标快照状态
        /// </summary>
        /// <param name="aimSnapshot">目标快照状态</param>
        /// <param name="fadeInTime">淡入时间，为0时立即切入</param>
        public void SwitchToSnapshot(AudioMixerSnapshot aimSnapshot,float fadeInTime)
        {
            aimSnapshot.TransitionTo(fadeInTime);
            currentShot = aimSnapshot;
        }
    }
}