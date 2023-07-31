using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class AudioPlayer : MonoBehaviour
{
    public AudioManager audioManager;
    private string audioFilePath; // Kaydedilen ses dosyasýnýn tam dosya yolu

    public void PlayRecordedAudio(string name)
    {
        audioFilePath = (Application.persistentDataPath + "/" + name + ".wav");
        audioManager.PlayRecordedAudio(audioFilePath);
    }

    public void StopPlayback()
    {
        audioManager.StopAudio();
    }
}