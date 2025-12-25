using System;
using System.Collections.Generic;
using UnityEngine;

namespace DreemurrStudio.AudioSystem
{
    /// <summary>
    /// 环境音播放信息
    /// </summary>
    [System.Serializable]
    public struct AmbientInfo
    {
        [Tooltip("项目中名称：使用此名称来播放")]
        public string name;
        [Tooltip("对应的音频源")]
        public AudioClip clip;
        [Range(-1f,1f)][Tooltip("音量调整，源音频音量过大过小时使用")]
        public float volumeAdjust;
        
        /// <summary>
        /// 音乐长度
        /// </summary>
        public float Length => clip.length;
        /// <summary>
        /// 音乐的音量
        /// </summary>
        public float Volume => volumeAdjust + 1f;
        
        public AmbientInfo(AudioClip clip)
        {
            this.clip = clip;
            this.name = clip.name;
            this.volumeAdjust = 0f;
        }
        public AmbientInfo(string name, AudioClip clip, float volumeAdjust)
        {
            this.name = name;
            this.clip = clip;
            this.volumeAdjust = volumeAdjust;
        }
    }
    /// <summary>
    /// 环境音注册表
    /// </summary>
    [CreateAssetMenu(fileName = "AmbientRegistrySO", menuName = "数据/音频/环境音注册表",order = 1)]
    public class AmbientRegistrySO : ScriptableObject
    {
        [SerializeField][Tooltip("背景音乐注册信息")]
        private List<AmbientInfo> ambientInfos = new List<AmbientInfo>();

        /// <summary>
        /// 获取要使用的环境音
        /// </summary>
        /// <param name="bgmName">环境音的项目名</param>
        /// <returns></returns>
        /// <exception cref="Exception">未找到对应名的环境音</exception>
        public AmbientInfo GetAmbientInfo(string bgmName)
        {
            var i = ambientInfos.Find(i => i.name == bgmName);
            if (i.name == "") throw new Exception("未注册的环境音！");
            return i;
        }
    }
}
