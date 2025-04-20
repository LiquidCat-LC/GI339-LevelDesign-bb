using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class EndCutscene : MonoBehaviour
{
    public AudioSource[] audioSource;
    public AudioClip[] newClip;

    public Image fadeImage;
    public TMP_Text theEnd;

    public IEnumerator PlaySoundRepeatedly(float delayBetweenPlays, int repeatCount,int sound)
    {
        yield return  StartCoroutine(PlaySoundCoroutine(delayBetweenPlays, repeatCount,sound));
    }

    private IEnumerator PlaySoundCoroutine(float delayBetweenPlays, int repeatCount,int sound)
    {
        for (int i = 0; i < repeatCount; i++)
        {
            audioSource[sound].Play();
            yield return new WaitForSeconds(audioSource[sound].clip.length + delayBetweenPlays);
        }
    }
    private IEnumerator EndCoroutine()
    {
        yield return new WaitForSeconds(3f);
        yield return PlaySoundRepeatedly(0.8f,4,0);
        yield return new WaitForSeconds(1f);
        yield return PlaySoundRepeatedly(0.02f,10,1);
        yield return new WaitForSeconds(3f);
        audioSource[2].Play();
        theEnd.gameObject.SetActive(true);

    }
     private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        float fadeDuration = 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        fadeImage.color = new Color(color.r, color.g, color.b, 1f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(FadeIn());
            StartCoroutine(EndCoroutine());
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
