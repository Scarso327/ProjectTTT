using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashFade : MonoBehaviour
{
    public RawImage splashImage;
    public Text VideoInfo;

    IEnumerator Start()
    {
        splashImage.canvasRenderer.SetAlpha(0.0F);
        VideoInfo.canvasRenderer.SetAlpha(0.0F);

        splashImage.CrossFadeAlpha(1.0F, 1.5F, false);
        yield return new WaitForSeconds(2.5F);
        splashImage.CrossFadeAlpha(0.0F, 2.5F, false);
        yield return new WaitForSeconds(2.5F);
        VideoInfo.CrossFadeAlpha(1.0F, 1.5F, false);
        yield return new WaitForSeconds(2.5F);
        VideoInfo.CrossFadeAlpha(0.0F, 2.5F, false);
        yield return new WaitForSeconds(2.5F);

        SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        { StopCoroutine("Start"); SceneManager.LoadScene("MainMenu"); }

    }
}
