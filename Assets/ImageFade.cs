using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ImageFade : MonoBehaviour
{
    //SpriteRenderer r;
    public GameObject loadingBar;
    //public Material fadeMat;
    Image i;
    // Start is called before the first frame update
    void Start()
    {
        //r = GetComponent<SpriteRenderer>();
        i = GetComponent<Image>();
        
        StartCoroutine(FadeIn(2));
    }

    IEnumerator FadeIn(float duration)
    {
        float time = 0;
        while (time < duration)
        {
            Vector3 newScale = loadingBar.transform.localScale;
            newScale.x += .005f;
            Vector3 newPos = loadingBar.transform.position;
            newPos.x += .0025f;
            loadingBar.transform.position = newPos;
            loadingBar.transform.localScale = newScale;

            time += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(FadeOut(2));
    }

    IEnumerator FadeOut(float duration)
    {
        float startValue = i.tintColor.a;
        float time = 0;
        while (time < duration)
        {
            Color newColor = i.tintColor;
            newColor.a = Mathf.Lerp(startValue, 1, time / duration);
            
            i.tintColor = newColor;
            time += Time.deltaTime;
            yield return null;
        }
        //StartCoroutine(FadeOut(2));
    }

    //IEnumerator FadeIn(float duration)
    //{
    //    float startValue = r.color.a;
    //    float time = 0;
    //    while (time < duration)
    //    {
    //        Color newColor = r.color;
    //        newColor.a = Mathf.Lerp(startValue, 1, time / duration);
    //        Vector3 newScale = loadingBar.transform.localScale;
    //        newScale.x += .005f;
    //        Vector3 newPos = loadingBar.transform.position;
    //        newPos.x += .0025f;
    //        loadingBar.transform.position = newPos;
    //        loadingBar.transform.localScale = newScale;
    //        r.color = newColor;
    //        time += Time.deltaTime;
    //        yield return null;
    //    }
    //    StartCoroutine(FadeOut(2));
    //}

    //IEnumerator FadeOut(float duration)
    //{
    //    loadingBar.GetComponent<MeshRenderer>().material = fadeMat;
    //    float startValue = r.color.a;
    //    float time = 0;
    //    while (time < duration)
    //    {
    //        Color newColor = r.color;
    //        Color barColor = loadingBar.GetComponent<MeshRenderer>().materials[0].color;
    //        newColor.a = Mathf.Lerp(startValue, 0, time / duration);
    //        barColor.a = newColor.a;
    //        Debug.Log(barColor.a);
    //        loadingBar.GetComponent<MeshRenderer>().materials[0].color = barColor;
    //        r.color = newColor;
    //        time += Time.deltaTime;
    //        yield return null;
    //    }
    //    SceneManager.LoadScene("Main Menu");
    //}

    // Update is called once per frame
    void Update()
    {
        
    }
}
