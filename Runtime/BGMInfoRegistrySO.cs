using System;
using System.Collections.Generic;
using UnityEngine;

namespace DreemurrStudio.AudioSystem
{   
    /// <summary>
    /// 背景音乐播放信息
    /// </summary>
    [System.Serializable]
    public struct BGMInfo
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
        public BGMInfo(AudioClip clip)
        {
            this.clip = clip;
            this.name = clip.name;
            this.volumeAdjust = 0f;
        }
        public BGMInfo(string name, AudioClip clip, float volumeAdjust)
        {
            this.name = name;
            this.clip = clip;
            this.volumeAdjust = volumeAdjust;
        }
    }
    
    /// <summary>
    /// 背景音乐播放信息注册表
    /// 将需要播放的背景音乐先在此表中进行注册
    /// </summary>
    [CreateAssetMenu(fileName = "BGMInfoRegistry", menuName = "数据/音频/背景音乐表", order = 0)]
    public class BGMInfoRegistrySO : ScriptableObject
    {
        [SerializeField][Tooltip("背景音乐注册信息")]
        private List<BGMInfo> bgmInfos = new List<BGMInfo>();

        /// <summary>
        /// 获取要使用的背景音乐
        /// </summary>
        /// <param name="bgmName">背景音乐的项目名</param>
        /// <returns></returns>
        /// <exception cref="Exception">未找到对应名的背景音乐</exception>
        public BGMInfo GetBGMInfo(string bgmName)
        {
            var bi = bgmInfos.Find(bi => bi.name == bgmName);
            if (bi.name == "") throw new Exception("未注册的背景音乐！");
            return bi;
        }
    }
}