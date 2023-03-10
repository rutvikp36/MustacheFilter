using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine.Networking;
using System.Data;
using System;

public class DatabaseScript : MonoBehaviour
{

    private static DatabaseScript instance;

    public static DatabaseScript Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<DatabaseScript>() as DatabaseScript;
            return instance;
        }
    }


    string filepath;

    IDbConnection dbCon;

    public int lastId = 0;
    public List<VideoDataModel> videoData = new List<VideoDataModel>();

    private void Awake()
    {
        filepath = Application.persistentDataPath + "/" + "SavedVideosData.sqlite3";
        dbCon = CreateAndOpenDatabse();
        readTableData();
        MenuScript.Instance.populateVideos();
    }
    public IDbConnection CreateAndOpenDatabse()
    {

        string connection = "URI=file:" + filepath;
        
        if (!File.Exists(filepath))
        {
            Debug.Log("Check in file found");
            var load = File.Create(filepath);
            load.Close();
            
        }
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();

        IDbCommand dbCommandCreateTable = dbcon.CreateCommand();
        dbCommandCreateTable.CommandText = "CREATE TABLE IF NOT EXISTS VideoData (id INTEGER PRIMARY KEY, videoPath TEXT, previewImage BLOB, tag TEXT , videoDutation TEXT, m_id INTEGER, previewImageWidth INTEGER, previewImageHeight INTEGER)";
        dbCommandCreateTable.ExecuteReader();
        return dbcon;



    }

    public void readTableData()
    {
        try
        {
            IDbCommand cmnd_read = dbCon.CreateCommand();
            IDataReader reader;
            string query = "SELECT * FROM VideoData";
            cmnd_read.CommandText = query;
            reader = cmnd_read.ExecuteReader();
            while (reader.Read())
            {
                byte[] img = (byte[])reader["previewImage"];
                Texture2D tex = new Texture2D(reader.GetInt32(6), reader.GetInt32(7), TextureFormat.RGBA32, false);
                tex.LoadRawTextureData(img);
                tex.Apply();
                videoData.Add(new VideoDataModel(reader.GetInt32(0), reader.GetInt32(5), reader.GetString(1), tex, reader.GetString(3), reader.GetString(4), reader.GetInt32(6), reader.GetInt32(7)));
                lastId = reader.GetInt32(0) + 1;
            }
           
        } catch (Exception e)
        {
            MenuScript.Instance.LogMessage.text += e.StackTrace;
            MenuScript.Instance.LogMessage.text += Environment.NewLine;

            MenuScript.Instance.LogMessage.text += e.Message;
            MenuScript.Instance.LogMessage.text += Environment.NewLine;
        }
    }

    public void insertVideoData(VideoDataModel vidData)
    {
        
        IDbCommand dbcmd = dbCon.CreateCommand();

        


        byte[] preImageBytes = vidData.previewImage.GetRawTextureData();
        Debug.Log(System.Text.Encoding.UTF8.GetString(preImageBytes));

        string sqlQuery = "INSERT INTO VideoData (id, videoPath, previewImage, tag, videoDutation, m_id, previewImageWidth, previewImageHeight) VALUES ( " + vidData.id + ", '" + vidData.videoPath + "', " + "@BlobContent" + ", '" + vidData.tag + "', '" + vidData.videoDuration + "', "+ vidData.m_id+", " +vidData.preImageWidth + ", " + vidData.preImageHeight + ")";
        SqliteParameter setParam = new SqliteParameter("@BlobContent", preImageBytes);
        dbcmd.Parameters.Add(setParam);
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteReader();

    }
    
    public void updateTag(int id, string tag)
    {
        IDbCommand dbcmd = dbCon.CreateCommand();
        string sqlQuery = string.Format("UPDATE VideoData set tag = @tag where id = " + id);
        SqliteParameter P_update = new SqliteParameter("@tag", tag);
        dbcmd.Parameters.Add(P_update);
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
    }
    private void OnApplicationQuit()
    {
        dbCon.Close();
    }
}
