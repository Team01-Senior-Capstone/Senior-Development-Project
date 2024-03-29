using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ImageFade : MonoBehaviour
{
    public Slider slider;
    SpriteRenderer r;
    public Image i;
    public GameObject loadingBar;
    // Start is called before the first frame update
    void Start()
    {
        //r = GetComponent<SpriteRenderer>();
        //StartCoroutine(FadeIn(2));
        
    }

    IEnumerator FadeIn(float duration)
    {
        float startValue = r.color.a;
        float time = 0;
        while (time < duration)
        {
            Color newColor = r.color;
            newColor.a = Mathf.Lerp(startValue, 1, time / duration);
            Vector3 newScale = loadingBar.transform.localScale;
            newScale.x += .005f;
            Vector3 newPos = loadingBar.transform.position;
            newPos.x += .0025f;
            loadingBar.transform.position = newPos;
            loadingBar.transform.localScale = newScale;
            r.color = newColor;
            time += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(FadeOut(2));
    }

    IEnumerator FadeOut(float duration)
    {
        float startValue = i.color.a;
        float time = 0;
        while (time < duration)
        {
            //Color newColor = r.color;
            //Color barColor = loadingBar.GetComponent<MeshRenderer>().material.color;
            Color a = i.color;
            a.a = Mathf.Lerp(startValue, 1, time / duration);
            ///barColor.a = newColor.a;
            //loadingBar.GetComponent<MeshRenderer>().material.color = barColor;
            //r.color = newColor;
            i.color = a;
            time += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene("Game Video");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        slider.value += .01f;
        if(slider.value > .999f)
        {
            //SceneManager.LoadScene("Main Menu");
            StartCoroutine(FadeOut(2));
        }
    }

}