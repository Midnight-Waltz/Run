using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainGenerator;

namespace TerrainGenerator
{
    public struct TrapType
    {
        public string typeName;
        public GameObject[] optionPrefabs;
        public int initWeight;
        public int minCount;
        public int maxCount;
        public int nowWeight;
        public string[] avoidPath;

        public TrapType(string typeName, GameObject[] optionPrefabs, int initWeight, int maxCount = 1, int minCount = 1, string[] avoidPath=null)
        {
            this.typeName = typeName;
            this.optionPrefabs = optionPrefabs;
            this.initWeight = initWeight;
            this.minCount = minCount;
            this.maxCount = maxCount;
            this.nowWeight = initWeight;
            this.avoidPath = avoidPath;
        }
    }

    public class DectorAndTrapOnRoadbedGenertor : MonoBehaviour
    {
        [SerializeField]
        public GameObject[] grassPrefabs;
        [SerializeField]
        public GameObject[] slabStonePrefabs;
        [SerializeField]
        public GameObject[] trapBladePrefabs;
        [SerializeField]
        public GameObject[] trapDispenserPrefabs;
        [SerializeField]
        public GameObject[] trapHangSawPrefabs;
        [SerializeField]
        public GameObject[] trapPressurePrefabs;
        [SerializeField]
        public GameObject[] trapSawPrefabs;
        [SerializeField]
        public GameObject[] trapSpikePrefabs;
        [SerializeField]
        public GameObject[] trapStoneObstaklePrefabs;

        private TrapType[] trapList;
        private int grassWeight = 4;
        private int slabStoneWeight = 1;
        private int beforeTrapType = 0;
        // Start is called before the first frame update
        void Start()
        {
            this.trapList = new TrapType[]
            {
                new TrapType("dector", null, 50, 5),
                new TrapType("trapBlade", this.trapBladePrefabs, 15, 3),
                new TrapType("trapDispenser", this.trapDispenserPrefabs, 3, 3),
                new TrapType("trapHangSaw", this.trapHangSawPrefabs, 2, 1),
                new TrapType("trapPressure", this.trapPressurePrefabs, 3, 2),
                new TrapType("trapSaw", this.trapSawPrefabs, 5, 3),
                new TrapType("trapSpike", this.trapSpikePrefabs, 5, 3),
                new TrapType("trapStoneObstakle", this.trapStoneObstaklePrefabs, 2, 2)
            };
        }

        int GetTrapType(string pathName)
        {
            int totalWeight = 0;
            for (int i = 0; i < this.trapList.Length; i++)
            {
                totalWeight += this.trapList[i].nowWeight;
            }
            int randomWeight = UnityEngine.Random.Range(0, totalWeight);
            int weightSum = 0;
            for (int i = 0; i < this.trapList.Length; i++)
            {
                weightSum += this.trapList[i].nowWeight;
                if (randomWeight < weightSum)
                {
                    return i;
                }
            }

            return 0;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void GenerateDector(Vector3 position, GameObject pathObject, int decorNum, int pathPointsCount)
        {
            Vector3[] positionList = new Vector3[]
            {
                position + pathObject.transform.right * 10.0f,
                position,
                position - pathObject.transform.right * 10.0f
            };
            for(int i = 0; i < 3; i++)
            {
                int dectorRandom = UnityEngine.Random.Range(0, 10);
                if (dectorRandom < this.grassWeight)
                {
                    InstantiateDectorAndTrap(this.grassPrefabs[UnityEngine.Random.Range(0, this.grassPrefabs.Length)], pathObject, positionList[i], decorNum, pathPointsCount);
                }
                else if(dectorRandom < this.grassWeight + this.slabStoneWeight)
                {
                    InstantiateDectorAndTrap(this.slabStonePrefabs[UnityEngine.Random.Range(0, this.slabStonePrefabs.Length)], pathObject, positionList[i], decorNum, pathPointsCount);
                }
            }
        }

        void GenerateTrap(Vector3 position, GameObject pathObject, int trapType, int decorNum, int pathPointsCount)
        {
            Vector3[] positionList = new Vector3[]
            {
                position + pathObject.transform.right * 10.0f,
                position,
                position - pathObject.transform.right * 10.0f
            };
            int num = 0;
            for (int i = 0; i < 3; i++) {
                if (UnityEngine.Random.Range(0, 2) == 1)
                {
                    InstantiateDectorAndTrap(this.trapList[trapType].optionPrefabs[UnityEngine.Random.Range(0, this.trapList[trapType].optionPrefabs.Length)], pathObject, positionList[i], decorNum, pathPointsCount);
                    num++;
                }
                if(num == 2)
                {
                    break;
                }
            }
        }

        void InstantiateDectorAndTrap(GameObject Prefab, GameObject pathObject, Vector3 position, int decorNum, int pathPointsCount)
        {
            Quaternion originalRotation = pathObject.transform.Find("LinkPoint").rotation;
            if (pathObject.name.Contains("SlopDown"))
            {
                Quaternion addRotation = Quaternion.Euler(18, 0, 0);
                originalRotation *= addRotation;
            }
            else if (pathObject.name.Contains("Slope"))
            {
                Quaternion addRotation = Quaternion.Euler(-18, 0, 0);
                originalRotation *= addRotation;
            }
            else if (pathObject.name.Contains("Corner"))
            {
                float y_rotation = originalRotation.eulerAngles.y - pathObject.transform.rotation.eulerAngles.y;
                y_rotation = GeneratorTool.NormalizeAngle(y_rotation);
                Debug.Log("pathName: "+pathObject.name + "y_rotation: " + y_rotation);

                originalRotation = pathObject.transform.rotation;
                Quaternion addRotation = Quaternion.Euler(0, y_rotation * (decorNum / (pathPointsCount + 2.0f)), 0);
                originalRotation *= addRotation;
            }
            Instantiate(Prefab, position, originalRotation);
        }

        public void GenerateDecorAndTrapOnRoadbed(GameObject pathObject)
        {
            if(pathObject.name.Contains("Bridge") || pathObject.name.Contains("PathPlatform"))
            {
                return;
            }
            List<Vector3> pathPoints = GeneratorTool.GetPathPoint(pathObject, new Vector2(0, 0), new Vector2(0, 0), 12.0f);
            int decorNum = 0;
            while (decorNum < pathPoints.Count)
            {
                int trapType = GetTrapType(pathObject.name);
                int generateNum = UnityEngine.Random.Range(this.trapList[trapType].minCount, Mathf.Min(this.trapList[trapType].maxCount, pathPoints.Count - decorNum));
                switch (trapType)
                {
                    case 0:
                        for(int i = 0; i < generateNum; i++)
                        {
                            GenerateDector(pathPoints[decorNum], pathObject, decorNum, pathPoints.Count);
                            decorNum += 1;
                        }
                        break;
                    case 1:
                    case 6:
                        for(int i = 0; i < generateNum; i++)
                        {
                            GenerateTrap(pathPoints[decorNum], pathObject, trapType, decorNum, pathPoints.Count);
                            decorNum += 1;
                        }
                        break;
                    case 2:
                        for (int i = 0; i < generateNum; i++)
                        {
                            decorNum += 1;
                        }
                        break;
                    case 3:
                    case 4:
                    case 7:
                        for(int i = 0; i < generateNum; i++)
                        {
                            InstantiateDectorAndTrap(this.trapList[trapType].optionPrefabs[UnityEngine.Random.Range(0, this.trapList[trapType].optionPrefabs.Length)], pathObject, pathPoints[decorNum], decorNum, pathPoints.Count);
                            decorNum += 1;
                        }
                        break;
                    case 5:
                        for (int i = 0; i < generateNum; i++)
                        {
                            InstantiateDectorAndTrap(this.trapList[trapType].optionPrefabs[UnityEngine.Random.Range(0, this.trapList[trapType].optionPrefabs.Length)], pathObject, pathPoints[decorNum], decorNum, pathPoints.Count);
                            decorNum += 1;
                        }
                        break;
                }
                
                this.trapList[beforeTrapType].nowWeight = this.trapList[beforeTrapType].initWeight;
                if( trapType != 0)
                {
                    this.trapList[trapType].nowWeight = 0;
                }
                this.beforeTrapType = trapType;
            }
        }
    }
}
