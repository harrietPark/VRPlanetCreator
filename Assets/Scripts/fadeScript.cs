using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class fadeScript : MonoBehaviour
{
    public bool startOpague = false;

    private float fadeAmount = 1f;

    public Image myFadeImage;

    private void Start()
    {
        Color fadeCol = myFadeImage.color;

        if (startOpague)
        {
            fadeCol.a = 1f;

            StartCoroutine(fadeIn());
        }
        else
        {
            fadeCol.a = 0f;

            myFadeImage.gameObject.SetActive(false);
        }

        myFadeImage.color = fadeCol;
    }

    public IEnumerator fadeOutToScene(int sceneIndex)
    {
        myFadeImage.gameObject.SetActive(true);

        Color fadeCol = myFadeImage.color;

        while(myFadeImage.color.a < 1)
        {
            fadeCol.a += fadeAmount * Time.deltaTime;

            myFadeImage.color = fadeCol;

            yield return new WaitForEndOfFrame();
        }

        //change the scene here

        SceneManager.LoadScene(sceneIndex);

        yield return null;
    }

    public IEnumerator fadeIn()
    {
        Color fadeCol = myFadeImage.color;

        while (myFadeImage.color.a > 0)
        {
            fadeCol.a -= fadeAmount * Time.deltaTime;

            myFadeImage.color = fadeCol;

            yield return new WaitForEndOfFrame();
        }

        myFadeImage.gameObject.SetActive(false);

        yield return null;
    }
}
