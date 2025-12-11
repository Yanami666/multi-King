using UnityEngine;

public class Book2DDialoguePlaySFXOnEnable : MonoBehaviour
{
    [Header("播放用的音源 / Shared audio source")]
    public AudioSource audioSource; // 建议所有对话图共用同一个 AudioSource

    [Header("这张对话出现时播放的音效 / Clip for THIS dialogue")]
    public AudioClip clip;

    [Header("随机音高（可选） / Randomize pitch (optional)")]
    public bool randomizePitch = false;
    public float minPitch = 0.95f;
    public float maxPitch = 1.05f;

    [Header("音量 / Volume")]
    [Range(0f, 1f)]
    public float volume = 1f;

    private void OnEnable()
    {
        if (audioSource == null || clip == null)
            return;

        // （1）先停掉之前正在播的任何声音
        audioSource.Stop();

        // （2）音高 & 音量设置
        if (randomizePitch)
        {
            audioSource.pitch = Random.Range(minPitch, maxPitch);
        }
        else
        {
            audioSource.pitch = 1f;
        }

        audioSource.volume = volume;

        // （3）切换到当前这张图的 clip，然后 Play
        audioSource.clip = clip;
        audioSource.Play();
    }
}