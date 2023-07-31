using UnityEngine;
using UnityEngine.Android;
using System.IO;
using UnityEngine.UI;

public class AudioRecorder : MonoBehaviour
{
    private string microphoneDevice;
    private AudioClip audioClip;
    private string audioPath;
    private bool isRecording = false;

    private void Start()
    {
        // Ýzinleri kontrol et
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
    }

    public void StartRecording(string name)
    {
        // Kaydedilecek sesin cihazdaki mikrofondan alýnmasý için ayarlarý yap
        microphoneDevice = Microphone.devices[0];
        audioClip = Microphone.Start(microphoneDevice, false, 10, 44100);

        // Sesin kaydedildiði dosya yolunu belirle
        audioPath = Path.Combine(Application.persistentDataPath, name + ".wav");

        //Debug.Log("Ses kaydediliyor: " + audioPath);

        isRecording = true;
    }

    public void StopRecording()
    {
        if (!isRecording)
            return;

        // Kaydý durdur
        Microphone.End(microphoneDevice);

        // Ses kaydýný diske kaydet
        SaveAudioClipToWav(audioPath, audioClip);

        isRecording = false;
    }

    private void SaveAudioClipToWav(string path, AudioClip clip)
    {
        // Ses kaydýný WAV formatýnda diske kaydet
        var samples = new float[clip.samples];
        clip.GetData(samples, 0);

        using (var fileStream = CreateEmpty(path))
        {
            ConvertAndWrite(fileStream, samples);
            WriteHeader(fileStream, clip);
        }
    }

    private FileStream CreateEmpty(string filePath)
    {
        // Boþ bir WAV dosyasý oluþtur
        var fileStream = new FileStream(filePath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < 44; i++) // WAV dosyasý baþlýk bilgisi
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }

    private void ConvertAndWrite(FileStream fileStream, float[] samples)
    {
        // Ses örneklemlerini dönüþtür ve yaz
        var byteArray = new byte[samples.Length * 2];

        for (int i = 0; i < samples.Length; i++)
        {
            var value = (short)(samples[i] * short.MaxValue);
            var bytes = System.BitConverter.GetBytes(value);
            fileStream.Write(bytes, 0, 2);
        }
    }

    private void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        // WAV dosyasý baþlýðýný yaz
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;
        var data = samples * channels;

        fileStream.Seek(0, SeekOrigin.Begin);
        var riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);
        var chunkSize = System.BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);
        var wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);
        var fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);
        var subChunk1 = System.BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);
        var audioFormat = System.BitConverter.GetBytes(1);
        fileStream.Write(audioFormat, 0, 2);
        var numChannels = System.BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);
        var sampleRate = System.BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);
        var byteRate = System.BitConverter.GetBytes(hz * channels * 2);
        fileStream.Write(byteRate, 0, 4);
        var blockAlign = System.BitConverter.GetBytes((ushort)(channels * 2));
        fileStream.Write(blockAlign, 0, 2);
        var bitsPerSample = System.BitConverter.GetBytes(16);
        fileStream.Write(bitsPerSample, 0, 2);
        var dataString = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(dataString, 0, 4);
        var subChunk2 = System.BitConverter.GetBytes(data);
        fileStream.Write(subChunk2, 0, 4);
    }
}
