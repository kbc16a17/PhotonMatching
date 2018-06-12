using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneTransit : SingletonMonoBehaviour<SceneTransit> {
    private float fadeAlpha = 0;
    private bool isFading = false;
    public Color fadeColor = Color.black;

    public void Awake() {
        if (this != Instance) {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public void OnGUI() {
        if (this.isFading) {
            this.fadeColor.a = this.fadeAlpha;
            GUI.color = this.fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }

    public void LoadScene(string scene, float interval) {
        StartCoroutine(transitScene(scene, interval));
    }

    private IEnumerator transitScene(string scene, float interval) {
        this.isFading = true;
        float time = 0;
        while (time <= interval) {
            this.fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        SceneManager.LoadScene(scene);

        time = 0;
        while (time <= interval) {
            this.fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.deltaTime;
            yield return 0;
        }

        this.isFading = false;
    }
}

