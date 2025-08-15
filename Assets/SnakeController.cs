using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class SnakeController : MonoBehaviour
{
    [SerializeField]
    public float speed;
    [SerializeField]
    private Segment[] segments = new Segment[3];
    [SerializeField]
    public float rotate_speed;
    [SerializeField]
    private DistanceConstrain distanceConstrain;
    [SerializeField]
    private RenderSnake renderSnake;
    public void RotateOnAxis()
    {
        // Поточний кут обертання
        float angle = rotate_speed * Input.GetAxis("Horizontal") * Time.deltaTime;

        // Вектор від центру осі до цього сегмента
        Vector2 dir = segments[0].GetPosition() - segments[1].GetPosition();

        // Обчислюємо новий напрямок після обертання
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        Vector2 rotatedDir = new Vector2(
            dir.x * cos - dir.y * sin,
            dir.x * sin + dir.y * cos
        );

        // Встановлюємо нову позицію
        segments[0].SetPosition(segments[1].GetPosition() + rotatedDir);
        segments[0].SetDirection(segments[0].GetPosition() - segments[1].GetPosition());
    }
    public void AddSegment(Segment segment)
    {
        List<Segment> segments_list = segments.ToList();
        segments_list.Add(segment);
        segments = segments_list.ToArray();
        distanceConstrain.SetPoints(segments);
    }
    public void RemoveSegment(Segment segment)
    {
        List<Segment> segments_list = segments.ToList();
        segments_list.Remove(segment);
        segments = segments_list.ToArray();
        distanceConstrain.SetPoints(segments);
    }
    private void Start()
    {
        segments[0] = new Segment(0.5f, new Vector2(-0.3370109f, -0.06301107f));
        segments[1] = new Segment(0.5f, new Vector2(0.77f, -0.06301107f));
        segments[2] = new Segment(0.5f, new Vector2(1.85f, -0.06301107f));
        distanceConstrain.SetPoints(segments);
    }
    // Update is called once per frame
    void Update()
    {
        

        RotateOnAxis();
        float v_input = Input.GetAxis("Vertical");
        if (v_input < 0)
        {
            v_input = 0;
        }
        segments[0].SetPosition(segments[0].GetPosition() + (segments[0].GetPosition() - segments[1].GetPosition()) * speed * v_input * Time.deltaTime);
        renderSnake.DrawSnakeMesh(segments);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 lastPos = segments[segments.Count() - 1].GetPosition();
            Vector2 lastDir = segments[segments.Count() - 1].GetDirection();
            float last_radius = segments[segments.Count() - 1].GetRadius();
            Vector2 newPos = lastPos - lastDir * (last_radius * 2);
            Segment segment = new Segment(0.5f, newPos);
            AddSegment(segment);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            RemoveSegment(segments[segments.Count() - 1]);
        }
    }
}
