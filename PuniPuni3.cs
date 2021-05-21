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
        // ついていくジョイントの番号
        public int jointIndex { get; set; }
        // ついていくジョイントとの位置の差
        public Vector3 difference { get; set; }
    }
    private List<VertexInfo> vertexInfoList = new List<VertexInfo>();

    // Start is called before the first frame update
    void Start()
    {
        // メッシュ取得
        mesh = GetComponent<MeshFilter>().mesh;
        // 頂点座標リストを作成
        mesh.GetVertices(vertexList);
        // ジョイントリスト初期化
        InitJointList();
        // 頂点情報初期化
        InitVertexInfo();
    }
    private void FixedUpdate()
    {
        for(int i = 0; i < vertexInfoList.Count; i++)
        {
            VertexInfo vertexInfo = vertexInfoList[i];
            vertexList[i] = jointList[vertexInfo.jointIndex].localPosition - vertexInfo.difference;
        }
        mesh.SetVertices(vertexList);
    }

    /// <summary>
    /// 頂点情報初期化
    /// </summary>
    private void InitVertexInfo()
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
                    vertexInfo.jointIndex = j;
                    vertexInfo.difference = (jointList[j].localPosition - vertexList[i]);
                }
                
            }
            vertexInfoList[i] = vertexInfo;
        }
    }

    /// <summary>
    /// ジョイントリスト初期化
    /// </summary>
    private void InitJointList()
    {
        Vector3[] jointPoints =
        {
            new Vector3(0.0f, 0.0f, -jointSize),
            new Vector3(0.0f, 0.0f, jointSize),
            new Vector3(-jointSize, 0.0f, 0.0f),
            new Vector3(jointSize, 0.0f, 0.0f),


            new Vector3(-jointSize, 0.0f, -jointSize),
            new Vector3(jointSize, 0.0f, -jointSize),
            new Vector3(-jointSize, 0.0f, jointSize),
            new Vector3(jointSize, 0.0f, jointSize),
        };

        foreach (Vector3 point in jointPoints)
        {
            jointList.Add(CreateJoint(point));
        }
    }

    /// <summary>
    /// ジョイントオブジェクト生成
    /// </summary>
    /// <param name="pos">生成する位置</param>
    /// <returns>生成したジョイントのTransform</returns>
    private Transform CreateJoint(Vector3 pos)
    {
        GameObject obj0 = new GameObject("JointObj");
        GameObject obj = Instantiate(obj0);
        obj.transform.SetParent(transform);
        obj.transform.localPosition = pos;
        obj.transform.localRotation = Quaternion.identity;

        SphereCollider sc = obj.AddComponent<SphereCollider>();
        sc.radius = 0.01f;
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
