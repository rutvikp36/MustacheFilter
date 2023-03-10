using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARCore;
using UnityEngine.XR.ARSubsystems;
using Unity.Collections;

public class MustachePosition : MonoBehaviour
{

    private static MustachePosition instance;

    public static MustachePosition Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MustachePosition>() as MustachePosition;
            return instance;
        }
    }

    [SerializeField]
    List<GameObject> m_Prefab;

    /// <summary>
    /// Get or set the prefab which will be instantiated at each detected face region.
    /// </summary>
    public int m_id;

    ARFaceManager m_FaceManager;

    ARSessionOrigin m_Origin;

    public bool isMustacheChanged;

    NativeArray<ARCoreFaceRegionData> m_FaceRegions;

    Dictionary<TrackableId, GameObject> m_InstantiatedPrefabs;


    // Start is called before the first frame update
    void Start()
    {
        m_id = 0;
        isMustacheChanged = false;
        m_FaceManager = GetComponent<ARFaceManager>();
        m_Origin = GetComponent<ARSessionOrigin>();

        m_InstantiatedPrefabs = new Dictionary<TrackableId, GameObject>();

    }

    // Update is called once per frame
    void Update()
    {

        var subsystem = (ARCoreFaceSubsystem)m_FaceManager.subsystem;
        if (subsystem == null)
            return;

        foreach (var face in m_FaceManager.trackables)
        {

            GameObject go;
            if (!m_InstantiatedPrefabs.TryGetValue(face.trackableId,out go))
            {
                go = Instantiate(m_Prefab[m_id], m_Origin.trackablesParent);
                m_InstantiatedPrefabs.Add(face.trackableId, go);
            }
            else if(isMustacheChanged)
            {
                Destroy(go);
                go = Instantiate(m_Prefab[m_id], m_Origin.trackablesParent);
                m_InstantiatedPrefabs[face.trackableId] = go;
                isMustacheChanged = false;
            }
            subsystem.GetRegionPoses(face.trackableId, Allocator.Persistent, ref m_FaceRegions);      

            go.transform.localPosition =  m_FaceRegions[0].pose.position; //transform.TransformPoint(face.vertices[164]);
            go.transform.localRotation = m_FaceRegions[0].pose.rotation;
        }
        if(m_FaceManager.trackables.count == 0)
        {
            foreach(KeyValuePair<TrackableId,GameObject>goObj in m_InstantiatedPrefabs)
            {

                    Destroy(goObj.Value);
         
            }
            m_InstantiatedPrefabs = new Dictionary<TrackableId,GameObject>();
        }


    }
}
