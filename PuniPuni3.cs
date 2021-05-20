using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PuniPuni3 : MonoBehaviour
{
    [SerializeField] private float spring = 0.0f;
    [SerializeField] private float damper = 0.2f;
    [SerializeField] private float minDistance = 0.0f;
    [SerializeField] private float maxDistance = 0.0f;
    [SerializeField] private float jointSize = 1.0f;

    //　メッシュ本体
    private Mesh mesh = null;
    //　メッシュの頂点
    private List<Vector3> vertexList = new List<Vector3>();
    //　ジョイントリスト
    private List<Transform> jointList = new List<Transform>();

    //　頂点情報
    public struct VertexInfo
    {
        public int jointIndex { get; set; }
        public float magnitude { get; set; }
        public Vector3 test { get; set; }
    }
    private List<VertexInfo> vertexInfoList = new List<VertexInfo>();

    // Start is called before the first frame update
    void Start()
    {
        // メッシュ取得
        mesh = GetComponent<MeshFilter>().mesh;
        // 頂点座標リストを作成
        mesh.GetVertices(vertexList);
        // ジョイントリスト作成
        CreateJointList();
        // 各頂点がついていくジョイントをセットする
        SetJointPoint();
    }
    private void FixedUpdate()
    {
        for(int i = 0; i < vertexInfoList.Count; i++)
        {
            VertexInfo vertexInfo = vertexInfoList[i];
            vertexList[i] = jointList[vertexInfo.jointIndex].localPosition - vertexInfo.test;
        }
        mesh.SetVertices(vertexList);
        ////　連動装置とメッシュの頂点の座標を連動させる
        //foreach (KeyValuePair<int, int> keyValue in sameDic)
        //{
        //    vertexList[keyValue.Key] = jointList[keyValue.Value].localPosition;
        //}
        //mesh.SetVertices(vertexList);
    }

    private void SetJointPoint()
    {
        vertexInfoList = new List<VertexInfo>(new VertexInfo[vertexList.Count]);
        VertexInfo vertexInfo = new VertexInfo();
        for (int i = 0; i < vertexList.Count; i++)
        {
            for(int j = 0; j < jointList.Count; j++)
            {
                float magnitude = (jointList[j].localPosition - vertexList[i]).sqrMagnitude;
                if (magnitude <= jointSize)
                {
                    vertexInfo.magnitude = magnitude;
                    vertexInfo.jointIndex = j;
                    vertexInfo.test = (jointList[j].localPosition - vertexList[i]);
                }
                
            }
            vertexInfoList[i] = vertexInfo;
        }
    }

    /// <summary>
    /// ジョイントリスト作成
    /// </summary>
    private void CreateJointList()
    {
        Vector3[] jointPoints =
        {
            // 上の手前
            new Vector3(-jointSize, jointSize, -jointSize),    // 左
            new Vector3(jointSize, jointSize, -jointSize),     // 右
            // 上の奥
            new Vector3(-jointSize, jointSize, jointSize),
            new Vector3(jointSize, jointSize, jointSize),
            // 下の手前
            new Vector3(-jointSize, -jointSize, -jointSize),
            new Vector3(jointSize, -jointSize, -jointSize),
            // 下の奥
            new Vector3(-jointSize, -jointSize, jointSize),
            new Vector3(jointSize, -jointSize, jointSize),
        };

        foreach (Vector3 point in jointPoints)
        {
            jointList.Add(CreateJoint(point));
        }
    }

    private Transform CreateJoint(Vector3 pos)
    {
        GameObject obj0 = new GameObject("Interlocker");
        GameObject obj = Instantiate(obj0);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = pos;
        obj.transform.localRotation = Quaternion.identity;

        //SphereCollider sc = obj.AddComponent<SphereCollider>();
        //sc.radius = 0.01f;
        SpringJoint sj = obj.AddComponent<SpringJoint>();
        sj.connectedBody = transform.GetComponent<Rigidbody>();
        sj.spring = spring;
        sj.damper = damper;
        sj.minDistance = minDistance;
        sj.maxDistance = maxDistance;
        Destroy(obj0);

        return obj.transform;
    }
}
