using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoadGenerator{
    /// <summary>
    /// 路径类型
    /// </summary>
    public struct pathType
    {
        public string typeName;
        public GameObject[] optionPrefabs;
        public int initWeight;
        public int minCount;
        public int maxCount;
        public int nowWeight;

        public pathType(string typeName, GameObject[] optionPrefabs, int initWeight, int maxCount =1, int minCount=1)
        {
            this.typeName = typeName;
            this.optionPrefabs = optionPrefabs;
            this.initWeight = initWeight;
            this.minCount = minCount;
            this.maxCount = maxCount;
            this.nowWeight = initWeight;
        }

    }

    public class RoadGenerator : MonoBehaviour
    {
        private LinkedList<GameObject> pathLinkedList = new LinkedList<GameObject>();

        public LinkedList<GameObject> PathLinkedList
        {
            get { return pathLinkedList; }
        }

        private int pathMaxCount = 200;

        private float turnAngle = 0;

        [SerializeField]
        public GameObject[] pathStartPrefab;

        [SerializeField]
        public GameObject[] pathEndPrefab;

        [SerializeField]
        public GameObject[] pathStraightPrefab;

        [SerializeField]
        public GameObject[] pathCornerPrefab;

        [SerializeField]
        public GameObject[] pathCornerLeftPrefab;

        [SerializeField]
        public GameObject[] pathSlopePrefab;

        [SerializeField]
        public GameObject[] pathSlopeDownPrefab;

        [SerializeField]
        public GameObject[] pathPlatformPrefab;

        [SerializeField]
        public GameObject[] bridgeStart;

        [SerializeField]
        public GameObject[] bridgePart;

        [SerializeField]
        public GameObject[] bridgeEnd;

        private pathType[] pathTypes;

        // Start is called before the first frame update
        void Start()
        {
            pathTypes = new pathType[]{
                new pathType("Straight", pathStraightPrefab, 50, 6, 2),
                new pathType("Corner", pathCornerPrefab, 10, 2),
                new pathType("CornerLeft", pathCornerLeftPrefab, 10, 2),
                new pathType("Slope", pathSlopePrefab, 5, 6, 5),
                new pathType("SlopeDown", pathSlopeDownPrefab, 5, 6, 2),
                new pathType("Platform", pathPlatformPrefab, 5),
                new pathType("Bridge", bridgeStart, 10)
            };
            GeneratePaths();
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        int getPathType(string pathName)
        {
            if (pathName.Contains("Straight"))
            {
                return 0;
            }
            else if (pathName.Contains("CornerLeft"))
            {
                return 2;
            }
            else if (pathName.Contains("Corner"))
            {
                return 1;
            }
            else if (pathName.Contains("SlopeDown"))
            {
                return 4;
            }
            else if (pathName.Contains("Slope"))
            {
                return 3;
            }
            else if (pathName.Contains("Platform"))
            {
                return 5;
            }
            else if (pathName.Contains("Bridge"))
            {
                return 6;
            }
            return 0;
        }

        // 获取生成路径的类型
        int GetGeneratePathType()
        {
            int totalWeight = 0;
            for (int i = 0; i < pathTypes.Length; i++)
            {
                totalWeight += pathTypes[i].nowWeight;
            }
            int randomWeight = UnityEngine.Random.Range(0, totalWeight);
            int weightSum = 0;
            for (int i = 0; i < pathTypes.Length; i++)
            {
                weightSum += pathTypes[i].nowWeight;
                if (randomWeight < weightSum)
                {
                    return i;
                }
            }
            return 0;
        }

        // 生成起始路径方块
        void GenerateStartPath()
        {
            GameObject startPath = Instantiate(pathStartPrefab[UnityEngine.Random.Range(0, pathStartPrefab.Length)], new Vector3(0, 0, 0), Quaternion.identity);
            pathLinkedList.AddLast(startPath);
        }

        // 生成路径方块
        void GeneratePath()
        {
            int pathType = GetGeneratePathType();
            GameObject[] pathPrefabs = pathTypes[pathType].optionPrefabs;
            int minCount = pathTypes[pathType].minCount;
            int maxCount = pathTypes[pathType].maxCount;
            int generateCount = UnityEngine.Random.Range(minCount, maxCount);
            GameObject last_path = pathLinkedList.Last.Value;
            int lastPathType = getPathType(last_path.name);
            int pathCount = pathLinkedList.Count;
            switch (pathType)
            {
                case 0:
                case 3:
                case 4:
                case 5:
                    for (int j = 0;j < generateCount; j++)
                    {
                        // 获取last_path下LinkPoint object的位置
                        Transform linkPoint = last_path.transform.Find("LinkPoint");
                        // 实例化一个pathPrefab, 位置为linkPoint的位置, 旋转为linkPoint的旋转
                        GameObject path = Instantiate(pathPrefabs[UnityEngine.Random.Range(0, pathPrefabs.Length)], linkPoint.position, linkPoint.rotation);
                        pathLinkedList.AddLast(path);
                        last_path = pathLinkedList.Last.Value;
                    }
                    break;
                case 1:
                case 2:
                    int i = 0;
                    while(i < generateCount)
                    {
                        GameObject cornerPrefab = pathPrefabs[UnityEngine.Random.Range(0, pathPrefabs.Length)];
                        // 获取cornerPrefab下LinkPoint object的角度
                        Transform cornerLinkPoint = cornerPrefab.transform.Find("LinkPoint");
                        float y_rotation = cornerLinkPoint.rotation.eulerAngles.y;
                        y_rotation = y_rotation > 180 ? y_rotation - 360 : y_rotation;
                        if (Math.Abs(y_rotation + turnAngle) <= 90)
                        {
                            Transform linkPoint = last_path.transform.Find("LinkPoint");
                            GameObject path = Instantiate(cornerPrefab, linkPoint.position, linkPoint.rotation);
                            pathLinkedList.AddLast(path);
                            last_path = pathLinkedList.Last.Value;
                            turnAngle += y_rotation;
                            i++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case 6:
                    GameObject bridgeStartPrefab = bridgeStart[UnityEngine.Random.Range(0, bridgeStart.Length)];
                    Transform bridgeStartLinkPoint = bridgeStartPrefab.transform.Find("LinkPoint");
                    GameObject bridgeStartPath = Instantiate(bridgeStartPrefab, last_path.transform.Find("LinkPoint").position, last_path.transform.Find("LinkPoint").rotation);
                    pathLinkedList.AddLast(bridgeStartPath);
                    last_path = pathLinkedList.Last.Value;
                    for (int j = 0; j < UnityEngine.Random.Range(1, 5); j++)
                    {
                        GameObject bridgePartPrefab = bridgePart[UnityEngine.Random.Range(0, bridgePart.Length)];
                        Transform bridgePartLinkPoint = bridgePartPrefab.transform.Find("LinkPoint");
                        GameObject bridgePartPath = Instantiate(bridgePartPrefab, last_path.transform.Find("LinkPoint").position, last_path.transform.Find("LinkPoint").rotation);
                        pathLinkedList.AddLast(bridgePartPath);
                        last_path = pathLinkedList.Last.Value;
                    }
                    GameObject bridgeEndPrefab = bridgeEnd[UnityEngine.Random.Range(0, bridgeEnd.Length)];
                    Transform bridgeEndLinkPoint = bridgeEndPrefab.transform.Find("LinkPoint");
                    GameObject bridgeEndPath = Instantiate(bridgeEndPrefab, last_path.transform.Find("LinkPoint").position, last_path.transform.Find("LinkPoint").rotation);
                    pathLinkedList.AddLast(bridgeEndPath);
                    last_path = pathLinkedList.Last.Value;
                    break;
                default:
                    break;

            }
            if(pathLinkedList.Count < pathCount)
            {
                pathTypes[lastPathType].nowWeight = pathTypes[lastPathType].initWeight;
                pathTypes[pathType].nowWeight = 0;
            }
        }


        void GeneratePaths(){
            GenerateStartPath();
            int times = 0;
            while(pathLinkedList.Count < pathMaxCount && times < 200)
            {
                times++;
                GeneratePath();
            }
        }

    }
}

