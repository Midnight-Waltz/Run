using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerrainGenerator
{
    public class GeneratorTool : MonoBehaviour
    {
        static public float turnAngle = 0;
        static Dictionary<string, int> pathNameIndexDictionary = new Dictionary<string, int>
        {
            {"PathStraight1", 0},
            {"PathStraight2", 1},
            {"PathStraight3", 2},
            {"PathCorner1", 3},
            {"PathCorner2", 4},
            {"PathCorner3", 5},
            {"PathCorner4", 6},
            {"PathCorner5", 7},
            {"PathCorner6", 8},
            {"PathCornerLeft1", 9},
            {"PathCornerLeft2", 10},
            {"PathCornerLeft3", 11},
            {"PathCornerLeft4", 12},
            {"PathCornerLeft6", 13},
            {"PathPlatform", 14}
        };

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 获取路径点
        /// </summary>
        /// <param name="pathObject">路径对象</param>
        /// <param name="startOffset">开始位置偏移，下标0为左（正）右（负），下标1为前（正）后（负）</param>
        /// <param name="endOffset">结束位置偏移，格式同开始位置</param>
        /// <param name="interval">间隔</param>
        /// <returns></returns>
        static public List<Vector3> GetPathPoint(GameObject pathObject, Vector2 startOffset, Vector2 endOffset, float interval)
        {
            List<Vector3> pathPoints = new List<Vector3>();
            Transform linkPoint = pathObject.transform.Find("LinkPoint");
            float y_rotation = linkPoint.rotation.eulerAngles.y;
            y_rotation = y_rotation > 180 ? y_rotation - 360 : y_rotation;
            Vector3 start = pathObject.transform.position + pathObject.transform.forward * startOffset[1] + pathObject.transform.right * startOffset[0];
            Vector3 end = linkPoint.position + linkPoint.forward * endOffset[1] + linkPoint.right * endOffset[0];
            string pathType = pathObject.name;
            // 去除路径名称中的(Clone)
            pathType = pathType.Replace("(Clone)", "");
            int pathNameIndex = pathNameIndexDictionary.ContainsKey(pathType) ? pathNameIndexDictionary[pathType] : 0;
            switch(pathNameIndex)
            {
                case 0:
                    pathPoints = GetStraightPathPoint(start, end, interval, 3);
                    break;
                case 1:
                    pathPoints = GetStraightPathPoint(start, end, interval, 1);
                    break;
                case 2:
                    pathPoints = GetStraightPathPoint(start, end, interval, 2);
                    break;
                case 3:
                    pathPoints = GetCornerPathPoint(start, end, GeneratorTool.turnAngle - y_rotation, interval, new Vector2(1, 0));
                    break;
                case 4:
                case 6:
                case 7:
                case 8:
                case 10:
                case 12:
                case 13:
                    pathPoints = GetCornerPathPoint(start, end, GeneratorTool.turnAngle - y_rotation, interval, new Vector2(1, -1));
                    break;
                case 5:
                    pathPoints = GetCornerPathPoint(start, end, GeneratorTool.turnAngle - y_rotation, interval, new Vector2(2, 1));
                    break;
                case 9:
                    pathPoints = GetCornerPathPoint(start, end, GeneratorTool.turnAngle - y_rotation, interval, new Vector2(0, -2));
                    break;
                case 11:
                    pathPoints = GetCornerPathPoint(start, end, GeneratorTool.turnAngle - y_rotation, interval, new Vector2(-1, -2));
                    break;
                case 14:
                    break;
            }
            return pathPoints;
        }

        /// <summary>
        /// 获取弯道路径点
        /// <param name="start"> 起始点</param>
        /// <param name="end"> 终点</param>
        /// <param name="angleC"> 弯道角度</param>
        /// <param name="interval"> 间隔</param>
        /// <param name="scopeCorrection"> 范围修正</param>
        /// </summary>
        static List<Vector3> GetCornerPathPoint(Vector3 start, Vector3 end, float angleC, float interval, Vector2 scopeCorrection)
        {
            List<Vector3> pathPoints = new List<Vector3>();

            // 计算A到B的中点M
            Vector3 midPoint = (start + end) / 2;

            // 计算A到B的夹角C的弧度值
            float angleCRad = angleC * Mathf.Deg2Rad;

            // 计算弧的半径
            float radius = Vector3.Distance(start, midPoint) / Mathf.Sin(angleCRad / 2);

            // 计算圆心O的坐标
            Vector3 direction = (end - start).normalized;
            Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 center = midPoint + normal * radius * Mathf.Cos(angleCRad / 2);

            // 计算弧长
            float arcLength = angleCRad * radius;

            // 计算点的数量n
            int n = Mathf.CeilToInt(arcLength / interval);

            // 计算每个点的角度增量
            float angleIncrement = angleCRad / (n - 1);

            // 沿着弯道的圆弧，以指定的间隔距离生成路径点
            for (int i = (int)scopeCorrection[0]; i < n + (int)scopeCorrection[1]; i++)
            {
                float angle = i * angleIncrement - GeneratorTool.turnAngle * Mathf.Deg2Rad;
                Vector3 point = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                pathPoints.Add(point);
            }
            return pathPoints;
        }

        /// <summary>
        /// 获取直道路径点
        /// <param name="start"> 起始点</param>
        /// <param name="end"> 终点</param>
        /// <param name="interval"> 间隔</param>
        /// <param name="offset"> 偏移</param>"
        /// /// </summary>
        static List<Vector3> GetStraightPathPoint(Vector3 start, Vector3 end, float interval, float offset)
        {
            // 计算直道的长度
            float length = Vector3.Distance(start, end);
            // 计算直道的方向
            Vector3 direction = (end - start).normalized;
            // 计算直道的数量
            int n = Mathf.CeilToInt(length / interval);
            // 计算每个点的位置
            List<Vector3> pathPoints = new List<Vector3>();
            for (int i = 0; i < n; i++)
            {
                Vector3 point = start + direction * (i * interval + offset);
                pathPoints.Add(point);
            }
            return pathPoints;
        }
    }
}

