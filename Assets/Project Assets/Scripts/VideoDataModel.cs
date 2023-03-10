using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoDataModel
{
    public int id;
    public int m_id;
    public string videoPath;
    public string tag;
    public string videoDuration;
    public Texture2D previewImage;
    public int preImageWidth;
    public int preImageHeight;
    public VideoDataModel(int id, int m_id,string videoPath, Texture2D previewImage, string tag, string videoDuration, int width, int height)
    {
        this.id = id;
        this.m_id = m_id;
        this.videoPath = videoPath;
        this.previewImage = previewImage;
        this.videoDuration = videoDuration;
        this.tag = tag;
        this.preImageWidth = width;
        this.preImageHeight = height;
    }

}
