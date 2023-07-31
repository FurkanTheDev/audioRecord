using UnityEngine;
using System.Collections;
using System.IO;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRecordedAudio(string audioPath)
    {
        StartCoroutine(LoadAudio(audioPath));
    }

    private IEnumerator LoadAudio(string audioPath)
    {
        var audioLoadRequest = GetAudioClipFromFile(audioPath);
        yield return audioLoadRequest;

        if (audioLoadRequest != null)
        {
            audioSource.clip = audioLoadRequest;
            audioSource.Play();
        }
    }

    private AudioClip GetAudioClipFromFile(string path)
    {
        if (File.Exists(path))
        {
            var audioData = File.ReadAllBytes(path);

            // Ses dosyasýnýn örnekleme hýzý ve diðer parametrelerini ayarla
            var audioClip = AudioClip.Create("RecordedAudio", audioData.Length / 2, 1, 44100, false);
            audioClip.SetData(ConvertAudioData(audioData), 0);

            return audioClip;
        }
        else
        {
            Debug.LogError("Ses dosyasý bulunamadý: " + path);
            return null;
        }
    }

    private float[] ConvertAudioData(byte[] audioData)
    {
        var audioFloatData = new float[audioData.Length / 2];

        for (int i = 0; i < audioFloatData.Length; i++)
        {
            var sample = (short)(audioData[i * 2] | (audioData[i * 2 + 1] << 8));
            audioFloatData[i] = sample / 32768f;
        }

        return audioFloatData;
    }

        public void StopAudio()
    {
        audioSource.Stop();
    }
}