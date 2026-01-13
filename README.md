# Unity 音频系统模块 (Audio System Module)

## 概述

本模块为 Unity 项目提供了一套功能全面、结构清晰的音频解决方案，它将音频播放划分为背景音乐（BGM）、环境音（Ambient）和音效（SFX）三个独立的部分进行管理

通过使用 `ScriptableObject` 进行音频注册和 `AudioMixer` 进行音量控制，该系统旨在提供一个易于扩展、配置和使用的音频框架

> 推荐在导入时将包内的 `unitypackage` 文件导入到您的 Unity 项目中，以便根据项目需求进行自定义修改和管理

## 核心功能

*   **模块化管理**:
    *   **`BGMManager`**: 专用于背景音乐，支持循环、续播、淡入淡出和播放间隔
    *   **`AmbientManager`**: 专用于环境音，支持循环播放
    *   **`SFXManager`**: 专用于音效，使用对象池管理音源，支持 2D（全局）和 3D（世界）音效
    *   **`AudioManager`**: 作为总控制器，通过 `AudioMixer` 统一管理各类音频的音量
*   **基于 ScriptableObject 的注册表**: 所有音频（BGM, Ambient, SFX）都需要在对应的注册表 SO 文件中进行配置，便于美术和策划进行管理，无需修改代码
*   **高级音效控制**:
    *   **随机播放**: 支持为一个音效配置多个随机的音频片段，增加多样性
    *   **重复策略**: 可配置音效在短时间内重复触发时的播放策略（如忽略、重播等）
*   **对象池**: `SFXManager` 内置对象池，高效复用 `AudioSource` 实例，减少运行时动态创建和销毁的开销
*   **动画事件支持**: 提供了 `AnimationEvent_Audio` 脚本，可以方便地在动画剪辑中通过事件触发音效
*   **音频保活**: 内置 `keepAliveSource` 机制，通过播放静音来确保 Unity 音频系统始终处于激活状态，防止在无声后首次播放音频失败

---

## 依赖项

*   **DreemurrStudio.Utilities**: 本模块中的所有管理器（`AudioManager`, `BGMManager` 等）都继承自 `Singleton<T>` 泛型基类，该基类由 `DreemurrStudio.Utilities` 包提供。请确保您的项目中已导入该依赖包

---

## 快速开始

### 1. 场景设置

1.  **创建管理器**: 从包的Prefabs目录下，将 `AudioManager`预制体拖拽到场景中实例化
4.  **配置 BGM/Ambient Manager**:
    *   创建或修改默认的 `BGMInfoRegistrySO`、 `AmbientRegistrySO` 资产文件，并对应地在其中注册所有要使用的音频文件
    *   参照Manager组件中属性的提示信息进行额外配置
5.  **配置 SFXManager**:
    *   创建或修改默认的 `AudioSource` 预制体，一个用于 UI音效，一个用于世界音效
    *   创建或修改默认的 `SoundEffectRegistrySO` 资产文件，注册所有要使用的音效

### 2. 播放背景音乐和环境音

```csharp
using DreemurrStudio.AudioSystem;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    void Start()
    {
        // 播放名为 "MainTheme" 的背景音乐，并带1.5秒的淡入效果
        BGMManager.Instance.Play("MainTheme", 1.5f);

        // 播放名为 "Forest" 的环境音
        AmbientManager.Instance.Play("Forest");
    }

    public void EnterBossRoom()
    {
        // 切换到 Boss 战音乐
        BGMManager.Instance.Play("BossBattle");
    }
}
```

### 3. 播放音效

```csharp
using DreemurrStudio.AudioSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 在角色行走时播放脚步声
    public void PlayFootstepSound()
    {
        // "Footstep" 是在 SoundEffectRegistrySO 中注册的名字
        // 该音效会以世界坐标形式在玩家对象的位置播放
        SFXManager.Instance.PlayWorldSFX("Footstep", this.gameObject);
    }

    // 在打开UI时播放一个全局音效
    public void PlayUIClickSound()
    {
        // "UIClick" 是一个2D UI音效，不受位置影响
        SFXManager.Instance.PlayOverlaySFX("UIClick");
    }
}
```

### 4. 控制音量

```csharp
using DreemurrStudio.AudioSystem;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public Slider sfxSlider;

    void Start()
    {
        // 初始化Slider的值
        sfxSlider.value = AudioManager.Instance.SFXVolume;

        // 监听Slider值的变化
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetSFXVolume(float volume)
    {
        // volume 的范围是 0.0 到 1.0
        AudioManager.Instance.SFXVolume = volume;
    }
}
```