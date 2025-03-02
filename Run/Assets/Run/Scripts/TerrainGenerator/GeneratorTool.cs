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
        /// ��ȡ·����
        /// </summary>
        /// <param name="pathObject">·������</param>
        /// <param name="startOffset">��ʼλ��ƫ�ƣ��±�0Ϊ�������ң��������±�1Ϊǰ�������󣨸���</param>
        /// <param name="endOffset">����λ��ƫ�ƣ���ʽͬ��ʼλ��</param>
        /// <param name="interval">���</param>
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
            // ȥ��·�������е�(Clone)
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
        /// ��ȡ���·����
        /// <param name="start"> ��ʼ��</param>
        /// <param name="end"> �յ�</param>
        /// <param name="angleC"> ����Ƕ�</param>
        /// <param name="interval"> ���</param>
        /// <param name="scopeCorrection"> ��Χ����</param>
        /// </summary>
        static List<Vector3> GetCornerPathPoint(Vector3 start, Vector3 end, float angleC, float interval, Vector2 scopeCorrection)
        {
            List<Vector3> pathPoints = new List<Vector3>();

            // ����A��B���е�M
            Vector3 midPoint = (start + end) / 2;

            // ����A��B�ļн�C�Ļ���ֵ
            float angleCRad = angleC * Mathf.Deg2Rad;

            // ���㻡�İ뾶
            float radius = Vector3.Distance(start, midPoint) / Mathf.Sin(angleCRad / 2);

            // ����Բ��O������
            Vector3 direction = (end - start).normalized;
            Vector3 normal = Vector3.Cross(direction, Vector3.up).normalized;
            Vector3 center = midPoint + normal * radius * Mathf.Cos(angleCRad / 2);

            // ���㻡��
            float arcLength = angleCRad * radius;

            // ����������n
            int n = Mathf.CeilToInt(arcLength / interval);

            // ����ÿ����ĽǶ�����
            float angleIncrement = angleCRad / (n - 1);

            // ���������Բ������ָ���ļ����������·����
            for (int i = (int)scopeCorrection[0]; i < n + (int)scopeCorrection[1]; i++)
            {
                float angle = i * angleIncrement - GeneratorTool.turnAngle * Mathf.Deg2Rad;
                Vector3 point = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                pathPoints.Add(point);
            }
            return pathPoints;
        }

        /// <summary>
        /// ��ȡֱ��·����
        /// <param name="start"> ��ʼ��</param>
        /// <param name="end"> �յ�</param>
        /// <param name="interval"> ���</param>
        /// <param name="offset"> ƫ��</param>"
        /// /// </summary>
        static List<Vector3> GetStraightPathPoint(Vector3 start, Vector3 end, float interval, float offset)
        {
            // ����ֱ���ĳ���
            float length = Vector3.Distance(start, end);
            // ����ֱ���ķ���
            Vector3 direction = (end - start).normalized;
            // ����ֱ��������
            int n = Mathf.CeilToInt(length / interval);
            // ����ÿ�����λ��
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

