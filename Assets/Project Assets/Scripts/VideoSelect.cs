using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class VideoSelect : MonoBehaviour
{

    [SerializeField]
    RawImage PreviewImage;

    [SerializeField]
    TextMeshProUGUI DurationText;

    [SerializeField]
    TextMeshProUGUI TagText;

    public int vid;



    public Texture2D previewTex
    {
        set => PreviewImage.texture = value;
    }

    public string tagText
    {
        set => TagText.text = value;
    }

    public string duration
    {
        set => DurationText.text = value;
    }


    public void onPlayBackClicked()
    {
        MenuScript.Instance.onAddVidClicked();
        RecordingScript.Instance.startPlayback(DatabaseScript.Instance.videoData[vid].videoPath);
    }

    public void onEditTagClicked()
    {
        MenuScript.Instance.update_id = vid;
        MenuScript.Instance.openEditTagPopup();
    }

}
