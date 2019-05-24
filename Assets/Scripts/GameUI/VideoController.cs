using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Borrowed and modified from code shown in https://www.youtube.com/watch?v=zCq0Jt6m8BQ
// Source: YouTube
// Creator/Author: Mir Imad
// Video: "Unity Play Video On Canvas | Unity UI Video Player Component | Unity 5 Tutorial | Unity Video"
public class VideoController : MonoBehaviour
{
    public RawImage image;
    public VideoPlayer vidPlayer;
    public MainMenuManager mMManager;

    // Start is called before the first frame update
    void Start()
    {
        image.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(PlayVideo());
    }

    // Borrowed and modified from code shown in https://www.youtube.com/watch?v=zCq0Jt6m8BQ
    IEnumerator PlayVideo() {
        vidPlayer.Prepare();
        WaitForSeconds waitForSecs = new WaitForSeconds(1);

        while (!vidPlayer.isPrepared) {
            yield return waitForSecs;
            break;
        }

        image.texture = vidPlayer.texture;

        // stops or starts/turns on video player and display texture depending on if player selection screen is active
        // hopefully will stop/cut down on stuttering when playing video and using localized cameras at the same time
        if (mMManager.getCurrentPanel() == 1 || mMManager.selectionMenuPanel.activeSelf != true) {
            if (!vidPlayer.isPlaying)
                vidPlayer.Play();
            image.gameObject.SetActive(true);
        }     
        else {
            vidPlayer.Pause();
            image.gameObject.SetActive(false);
        }
    }
}
