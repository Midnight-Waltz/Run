using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainGenerator;

namespace TerrainGenerator {
    public struct DecorOnBothSidesDecor
    {
        public string typeName;
        public GameObject[] optionPrefabs;
        public int initWeight;
        public int minCount;
        public int maxCount;
        public int nowWeight;
        public int size;

        public DecorOnBothSidesDecor(string typeName, GameObject[] optionPrefabs, int initWeight, int size = 1, int maxCount = 1, int minCount = 1)
        {
            this.typeName = typeName;
            this.optionPrefabs = optionPrefabs;
            this.initWeight = initWeight;
            this.minCount = minCount;
            this.maxCount = maxCount;
            this.nowWeight = initWeight;
            this.size = size;
        }
    }

    public class DecorOnBothSidesGenertor : MonoBehaviour
    {
        [SerializeField]
        public GameObject[] sizeOnePrefabs;
        [SerializeField]
        public GameObject[] sizeTwoPrefabs;
        [SerializeField]
        public GameObject[] sizeThreePrefabs;

        private DecorOnBothSidesDecor[] decorList;
        private float interval = 20.0f;
        private int nowLeftDecorType = 0;
        private int nowRightDecorType = 0;


        void Start()
        {
            this.decorList = new DecorOnBothSidesDecor[]
            {
                new DecorOnBothSidesDecor("null", null, 50, 1, 4),
                new DecorOnBothSidesDecor("sizeOne", this.sizeOnePrefabs, 15, 1),
                new DecorOnBothSidesDecor("sizeTwo", this.sizeTwoPrefabs, 1, 2, 2),
                new DecorOnBothSidesDecor("sizeThree", this.sizeThreePrefabs, 1, 3)
            };
        }

        // Update is called once per frame
        void Update()
        {

        }

        int GetGeneratePathType(string pathName, int overplusNum)
        {
            int totalWeight = 0;
            int rangeNum = pathName.Contains("Corner") ? 2 : this.decorList.Length;
            for (int i = 0; i < rangeNum; i++)
            {
                totalWeight += this.decorList[i].nowWeight;
            }
            int randomWeight = UnityEngine.Random.Range(0, totalWeight);
            int weightSum = 0;
            for (int i = 0; i < rangeNum; i++)
            {
                weightSum += this.decorList[i].nowWeight;
                if (randomWeight < weightSum)
                {
                    return i;
                }
            }
            return 0;
        }

        void GenerateDecor(List<Vector3> pathPoint, GameObject pathObject, bool isLeft=true)
        {
            int decorNum = 0;
            while (decorNum < pathPoint.Count)
            {
                int decorType = GetGeneratePathType(pathObject.name, pathPoint.Count - decorNum);
                int maxDecorNum = Mathf.Min(Mathf.FloorToInt((pathPoint.Count - decorNum) / this.decorList[decorType].size), this.decorList[decorType].maxCount);
                switch (decorType)
                {
                    case 0:
                        decorNum += this.decorList[decorType].size * maxDecorNum;
                        break;
                    case 1:
                        for (int i = 0; i < maxDecorNum; i++)
                        {
                            Instantiate(this.decorList[decorType].optionPrefabs[UnityEngine.Random.Range(0, this.decorList[decorType].optionPrefabs.Length)], pathPoint[decorNum], pathObject.transform.Find("LinkPoint").rotation);
                            decorNum += this.decorList[decorType].size;
                        }
                        break;
                    case 2:
                        for (int i = 0; i < maxDecorNum; i++)
                        {
                            Instantiate(this.decorList[decorType].optionPrefabs[UnityEngine.Random.Range(0, this.decorList[decorType].optionPrefabs.Length)], (pathPoint[decorNum] + pathPoint[decorNum + 1])/2, pathObject.transform.Find("LinkPoint").rotation);
                            decorNum += this.decorList[decorType].size;
                        }
                        break;
                    case 3:
                        for (int i = 0; i < maxDecorNum; i++)
                        {
                            Instantiate(this.decorList[decorType].optionPrefabs[UnityEngine.Random.Range(0, this.decorList[decorType].optionPrefabs.Length)], (pathPoint[decorNum] + pathPoint[decorNum + 2]) / 2, pathObject.transform.Find("LinkPoint").rotation);
                            decorNum += this.decorList[decorType].size;
                        }
                        break;
                }
                decorList[decorType].nowWeight = 0;
                if (isLeft)
                {
                    decorList[this.nowLeftDecorType].nowWeight = decorList[this.nowLeftDecorType].initWeight;
                    this.nowLeftDecorType = decorType;
                }
                else
                {
                    decorList[this.nowRightDecorType].nowWeight = decorList[this.nowRightDecorType].initWeight;
                    this.nowRightDecorType = decorType;
                }
            }
        }

        // 生成两侧的装饰物
        public void GenerateDecorOnBothSidesDecor(GameObject pathObject)
        {
            List<Vector3> leftPathPoints = GeneratorTool.GetPathPoint(pathObject, new Vector2(32, 0), new Vector2(32, 0), this.interval);
            List<Vector3> rightPathPoints = GeneratorTool.GetPathPoint(pathObject, new Vector2(-32, 0), new Vector2(-32, 0), this.interval);
            GenerateDecor(leftPathPoints, pathObject);
            GenerateDecor(rightPathPoints, pathObject);
        }
    }

}