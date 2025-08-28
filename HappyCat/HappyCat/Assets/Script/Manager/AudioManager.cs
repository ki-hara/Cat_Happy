using UnityEngine;
using System.Collections.Generic;
using System;
using HC.Resource;
using System.Collections;

//[Serializable]
//public class 

/// <summary>
/// 싱글톤 오디오 매니저
/// BGM / SFX 따로 관리 + 풀링 시스템
/// </summary>
public class AudioManager : MonoBehaviour
{
    public enum E_AUDIO
    {
        MainBGM,
        Click1
    }
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;              // 배경음 전용
    public AudioSource sfxSourcePrefab;        // 효과음용 프리팹 (풀링할 기본 AudioSource)
    public int sfxPoolSize = 10;

    [Header("Volume")]
    [Range(0f, 1f)] public float bgmVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private Queue<AudioSource> sfxPool = new Queue<AudioSource>();

    private Coroutine bgmFadeCoroutine;

    private void Awake()
    {
        // 싱글톤 보장
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 풀 생성
        sfxPool.Enqueue(sfxSourcePrefab);
        for (int i = 0; i < sfxPoolSize - 1; i++)
        {
            AudioSource newSource = Instantiate(sfxSourcePrefab, transform);
            newSource.gameObject.SetActive(false);
            sfxPool.Enqueue(newSource);
        }
    }

    // 🔹 BGM 재생
    public async void PlayBGM(E_AUDIO audio, bool loop = true)
    {
        AudioClip clip = await LoadAddressableManager.LoadAudio<AudioClip>(audio.ToString());

        if (clip == null || bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }
    public async void PlayBGMWithFade(E_AUDIO audio, float fadeDuration = 1f)
    {
        AudioClip clip = await LoadAddressableManager.LoadAudio<AudioClip>(audio.ToString());
        if (clip == null) return;
        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeBGM(clip, fadeDuration));
    }

    private IEnumerator FadeBGM(AudioClip newClip, float duration)
    {
        if (bgmSource.isPlaying)
        {
            // 페이드 아웃
            float startVolume = bgmSource.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }
            bgmSource.Stop();
        }

        // 새 클립 교체 & 페이드 인
        bgmSource.clip = newClip;
        bgmSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0f, bgmVolume, t / duration);
            yield return null;
        }

        bgmSource.volume = bgmVolume;
    }

    // 🔹 BGM 정지
    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // 🔹 효과음 재생
    public async void PlaySFX(E_AUDIO audio)
    {
        AudioClip clip = await LoadAddressableManager.LoadAudio<AudioClip>(audio.ToString());
        if (clip == null) return;

        AudioSource source = GetSFXSource();
        source.clip = clip;
        source.volume = sfxVolume;
        source.gameObject.SetActive(true);
        source.Play();

        StartCoroutine(DisableAfterPlay(source));
    }

    private AudioSource GetSFXSource()
    {
        if (sfxPool.Count > 0)
        {
            return sfxPool.Dequeue();
        }
        else
        {
            // 풀 부족하면 새로 생성
            return Instantiate(sfxSourcePrefab, transform);
        }
    }

    private System.Collections.IEnumerator DisableAfterPlay(AudioSource source)
    {
        yield return new WaitWhile(() => source.isPlaying);
        source.gameObject.SetActive(false);
        sfxPool.Enqueue(source);
    }
}