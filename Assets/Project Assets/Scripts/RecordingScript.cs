using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARFoundation;
using System.IO;
using System;
using UnityEngine.XR.ARSubsystems;

public class RecordingScript : MonoBehaviour
{


    private static RecordingScript instance;

    public static RecordingScript Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<RecordingScript>() as RecordingScript;
            return instance;
        }
    }

    [SerializeField]
    ARSession m_Session;

    [SerializeField]
    ARCameraManager cameraManager;

/*    ArStatus? m_SetMp4DatasetResult;
    ArPlaybackStatus m_PlaybackStatus = (ArPlaybackStatus)(-1);
    ArRecordingStatus m_RecordingStatus = (ArRecordingStatus)(-1);*/

    public string m_Mp4Path;

    public Texture2D m_CameraTexture;

    public DateTime startRecTime;

    public TimeSpan duration;

    public string savedTag;



    [SerializeField]
    GameObject RecordingPanel;

    [SerializeField]
    GameObject PlaybackPanel;

    private void Awake()
    {
        m_Mp4Path = Application.persistentDataPath +  "/arcore-session-" +DatabaseScript.Instance.lastId +".mp4";

    }
    private void Update()
    {

    }

    public void onRecordClick()
    {
        try
        {
            if (m_Session.subsystem is ARCoreSessionSubsystem subsystem)
            {
                StartCoroutine(TakeScreenshot());
                var session = subsystem.session;
                using (var config = new ArRecordingConfig(session))
                {
                    config.SetMp4DatasetFilePath(session, m_Mp4Path);
                    config.SetRecordingRotation(session, 0);
                    startRecTime = DateTime.Now;
                    var status = subsystem.StartRecording(config);

                }

            }
        }
        catch (Exception e)
        {
            MenuScript.Instance.LogMessage.text += e.Message;
            MenuScript.Instance.LogMessage.text += Environment.NewLine;
        }
    }

    public void onStopRecordingClicked()
    {
        if (m_Session.subsystem is ARCoreSessionSubsystem subsystem)
        {
            var status = subsystem.StopRecording();
            duration = DateTime.Now - startRecTime;
           
        }

        MenuScript.Instance.openNewTagPopup();
        

    }


    public void saveRecording()
    {
        try
        {
            //duration = TimeSpan.Zero;
            VideoDataModel videoData = new(DatabaseScript.Instance.lastId, MustachePosition.Instance.m_id,m_Mp4Path, m_CameraTexture, savedTag, duration.ToString(),m_CameraTexture.width, m_CameraTexture.height);
            MenuScript.Instance.LogMessage.text += videoData.tag;
            MenuScript.Instance.LogMessage.text += Environment.NewLine;
           DatabaseScript.Instance.insertVideoData(videoData);
            DatabaseScript.Instance.videoData.Add(videoData);
            DatabaseScript.Instance.lastId++;
            m_Mp4Path = Application.persistentDataPath +  "/arcore-session-" + DatabaseScript.Instance.lastId + ".mp4";
            MenuScript.Instance.populateVideos();
        }
        catch (Exception e)
        {
            
            //MenuScript.Instance.LogMessage.text += e.StackTrace;
            MenuScript.Instance.LogMessage.text += e.Message;
            MenuScript.Instance.LogMessage.text += Environment.NewLine;
        }
    }

    
    IEnumerator TakeScreenshot()
    {

        yield return new WaitForSeconds(0.1f);

        try
        {
            m_CameraTexture = ScreenCapture.CaptureScreenshotAsTexture();
            MenuScript.Instance.LogImage.texture = m_CameraTexture;
            /*m_CameraTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            m_CameraTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            m_CameraTexture.Apply();
            MenuScript.Instance.LogMessage.text += "scrrenshot captured";
            MenuScript.Instance.LogMessage.text += Environment.NewLine;*/


            MenuScript.Instance.LogMessage.text += m_CameraTexture.format;
            MenuScript.Instance.LogMessage.text += Environment.NewLine;

        }
        catch (Exception e)
        {
            MenuScript.Instance.LogMessage.text += e.Message;
            MenuScript.Instance.LogMessage.text += Environment.NewLine;
        }

    }

    public void startPlayback(string m_Mp4Path)
    {
        if (m_Session.subsystem is ARCoreSessionSubsystem subsystem)
        {
            RecordingPanel.SetActive(false);
            PlaybackPanel.SetActive(true);
            var status = subsystem.StartPlayback(m_Mp4Path);
        }
    }

    public void onStopPlayBackClicked()
    {
        if (m_Session.subsystem is ARCoreSessionSubsystem subsystem)
        {

            var status = subsystem.StopPlayback();
            RecordingPanel.SetActive(true);

            PlaybackPanel.SetActive(false);

        }
    }

}
