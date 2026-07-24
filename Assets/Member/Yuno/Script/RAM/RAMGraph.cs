using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class RAMGraphUI : MaskableGraphic
{
    [Header("그래프 설정")]
    [SerializeField] private float updateInterval = 0.3f; // 데이터 갱신 속도 (초)
    [SerializeField] private float thickness = 2f;        // 선 두께 (픽셀)
    [SerializeField] private int maxDataPoints = 40;      // 화면에 담길 점 개수

    private Queue<float> _dataPoints = new Queue<float>();
    private float _timer = 0f;

    protected override void Start()
    {
        base.Start();
        // 시작 시 0값으로 초기화
        for (int i = 0; i < maxDataPoints; i++)
        {
            _dataPoints.Enqueue(0f);
        }
    }

    private void Update()
    {
        if (MemoryManager.Instance == null) return;

        _timer += Time.deltaTime;
        if (_timer >= updateInterval)
        {
            _timer = 0f;

            // 현재 램 비율(0.0 ~ 1.0)을 데이터 큐에 푸시
            float ratio = Mathf.Clamp01((float)MemoryManager.Instance.CurrentRAM / MemoryManager.Instance.maxRAM);
            _dataPoints.Enqueue(ratio);

            if (_dataPoints.Count > maxDataPoints)
            {
                _dataPoints.Dequeue();
            }

            // 그래프 새로고침
            SetVerticesDirty();
        }
    }

    // Canvas에 선(Mesh)을 직접 그려주는 유니티 UI 엔진 함수
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (_dataPoints == null || _dataPoints.Count < 2) return;

        Rect r = GetPixelAdjustedRect();
        float width = r.width;
        float height = r.height;

        float stepX = width / (maxDataPoints - 1);
        float[] points = _dataPoints.ToArray();

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 start = new Vector2(r.xMin + i * stepX, r.yMin + points[i] * height);
            Vector2 end = new Vector2(r.xMin + (i + 1) * stepX, r.yMin + points[i + 1] * height);

            DrawLineSegment(vh, start, end, thickness, color);
        }
    }

    private void DrawLineSegment(VertexHelper vh, Vector2 start, Vector2 end, float thickness, Color32 lineUtilColor)
    {
        Vector2 dir = (end - start).normalized;
        Vector2 normal = new Vector2(-dir.y, dir.x) * (thickness * 0.5f);

        UIVertex v1 = UIVertex.simpleVert;
        UIVertex v2 = UIVertex.simpleVert;
        UIVertex v3 = UIVertex.simpleVert;
        UIVertex v4 = UIVertex.simpleVert;

        v1.color = lineUtilColor; v1.position = start - normal;
        v2.color = lineUtilColor; v2.position = start + normal;
        v3.color = lineUtilColor; v3.position = end + normal;
        v4.color = lineUtilColor; v4.position = end - normal;

        int index = vh.currentVertCount;
        vh.AddVert(v1); vh.AddVert(v2); vh.AddVert(v3); vh.AddVert(v4);

        vh.AddTriangle(index, index + 1, index + 2);
        vh.AddTriangle(index, index + 2, index + 3);
    }
}