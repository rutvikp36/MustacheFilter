using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{

    private static MenuScript instance;

    public static MenuScript Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MenuScript>() as MenuScript;
            return instance;
        }
    }


    [SerializeField]
    GameObject VideoRowPrefab;

    [SerializeField]
    GameObject VideoRowContainer;

    [SerializeField]
    GameObject PopUp;

    [SerializeField]
    GameObject PopUpBackGround;

    [SerializeField]
    GameObject SavedVideosPanel;

    [SerializeField]
    ARCameraManager cameraManager;

    [SerializeField]
    TextMeshProUGUI tagText;

    [SerializeField]
    TextMeshProUGUI NoVidsText;




    [SerializeField]
    GameObject EditPopUp;

    [SerializeField]
    GameObject EditPopUpBackGround;


    public TextMeshProUGUI LogMessage;

    public RawImage LogImage;

    public int update_id =0;
    private void Awake()
    {
       
    }

    public void onSwapCameraClicked()
    {
        if(cameraManager.requestedFacingDirection == CameraFacingDirection.World)
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.User;
        } else
        {
            cameraManager.requestedFacingDirection = CameraFacingDirection.World;
        }
    }

    public void onMustacheSelect(int id)
    {
        MustachePosition.Instance.m_id = id;
        MustachePosition.Instance.isMustacheChanged = true;
    }

    public void onVidelListClicked()
    {
        if (DatabaseScript.Instance.videoData.Count == 0) NoVidsText.gameObject.SetActive(true);
        else NoVidsText.gameObject.SetActive(false);
        SavedVideosPanel.transform.localPosition = new Vector3(800f, SavedVideosPanel.transform.localPosition.y, SavedVideosPanel.transform.localPosition.z);
        SavedVideosPanel.SetActive(true);
        LeanTween.moveLocalX(SavedVideosPanel, 0f, 0.3f);
    }








    public void openNewTagPopup()
    {
        PopUpBackGround.GetComponentInChildren<TMP_InputField>().text = "";
        LeanTween.alpha(PopUp, 1f, 0f);
        PopUpBackGround.transform.localScale = Vector3.zero;
        PopUp.SetActive(true);
        LeanTween.alpha(PopUp, 1f, 0.1f);
        LeanTween.scale(PopUpBackGround, Vector3.one, 0.3f);
        PopUpBackGround.GetComponentInChildren<TMP_InputField>().ActivateInputField();
    }

    public void onSaveVideoClicked()
    {
        RecordingScript.Instance.savedTag = tagText.text;
        RecordingScript.Instance.saveRecording();
        closePopup();
    }

    public void closePopup()
    {
        LeanTween.alpha(PopUp, 0f, 0.4f);
        LeanTween.scale(PopUpBackGround, Vector3.zero, 0.3f).setOnComplete(()=> {
            PopUp.SetActive(false);
        });
    }



    public void openEditTagPopup()
    {
        EditPopUpBackGround.GetComponentInChildren<TMP_InputField>().text = DatabaseScript.Instance.videoData[update_id].tag;
        LeanTween.alpha(EditPopUp, 1f, 0f);
        EditPopUpBackGround.transform.localScale = Vector3.zero;
        EditPopUp.SetActive(true);
        LeanTween.alpha(EditPopUp, 1f, 0.1f);
        LeanTween.scale(EditPopUpBackGround, Vector3.one, 0.3f);
        EditPopUpBackGround.GetComponentInChildren<TMP_InputField>().ActivateInputField();
    }

    public void onSaveTagClicked()
    {
        DatabaseScript.Instance.updateTag(update_id, EditPopUpBackGround.GetComponentInChildren<TMP_InputField>().text);
        DatabaseScript.Instance.videoData[update_id].tag = EditPopUpBackGround.GetComponentInChildren<TMP_InputField>().text;
        closeEditPopup();
    }
    public void closeEditPopup()
    {
        LeanTween.alpha(EditPopUp, 0f, 0.4f);
        LeanTween.scale(EditPopUpBackGround, Vector3.zero, 0.3f).setOnComplete(() => {
            populateVideos();
            EditPopUp.SetActive(false);
        });
    }

    public void populateVideos()
    {

        foreach(Transform child in VideoRowContainer.transform)
        {
            Destroy(child.gameObject);
        }
        for(int i = DatabaseScript.Instance.videoData.Count - 1; i >=0; i--)
        {
            Debug.Log(DatabaseScript.Instance.videoData.Count);
            GameObject VideoRow = Instantiate(VideoRowPrefab,Vector3.zero,Quaternion.identity);
            VideoRow.transform.parent = VideoRowContainer.transform;
            VideoSelect vidRow = VideoRow.GetComponent<VideoSelect>();
            vidRow.tagText = DatabaseScript.Instance.videoData[i].tag;
            vidRow.previewTex = DatabaseScript.Instance.videoData[i].previewImage;
            vidRow.duration = DatabaseScript.Instance.videoData[i].videoDuration;
            vidRow.vid = DatabaseScript.Instance.videoData[i].id;
        }
    }




    public void onAddVidClicked()
    {
        LeanTween.moveLocalX(SavedVideosPanel, 800f, 0.3f).setOnComplete(() =>
        {
            SavedVideosPanel.SetActive(false);
        });
    }
}
