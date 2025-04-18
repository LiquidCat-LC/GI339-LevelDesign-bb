using UHFPS.Runtime;
using UnityEngine;
using System.Collections;

public class TriggerVFX : MonoBehaviour
{
    public JumpscareTrigger jumpscare;

    private AudioSource audioSource;
    public AudioClip[] soundClips;
    public float volume = 1.0f;

    private void Awake()
    {
        jumpscare = GetComponent<JumpscareTrigger>();
    }

    void Start()
    {
        jumpscare.TriggerJumpscare();

         if (soundClips != null && soundClips.Length > 0)
        {
            AudioClip randomClip = soundClips[Random.Range(0, soundClips.Length)];

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.clip = randomClip;
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void OnDestroy()
    {
        if (audioSource != null)
        {
            audioSource.Stop(); // หยุดเสียง
            Destroy(audioSource); // ลบ AudioSource ออกจาก Object
        }
    }
}
