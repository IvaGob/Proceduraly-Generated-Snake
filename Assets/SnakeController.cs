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
    private Segment[] segments = new Segment[5];
    [SerializeField]
    public float rotate_speed;
    [SerializeField]
    private DistanceConstrain distanceConstrain;
    [SerializeField]
    private RenderSnake renderSnake;
    [SerializeField]
    private LayerMask obstacle_mask;
    [SerializeField]
    private LayerMask apple_mask;
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
    public void AddSegmentAfterSegment(Segment last_segment, float new_radius)
    {
        Vector2 lastPos = last_segment.GetPosition();
        Vector2 lastDir = last_segment.GetDirection();
        float last_radius = last_segment.GetRadius();
        Vector2 newPos = lastPos - lastDir * (last_radius * 2);
        Segment segment = new Segment(new_radius, newPos);
        AddSegment(segment);
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
        segments[1] = new Segment(0.4f, new Vector2(0.77f, -0.06301107f));
        segments[2] = new Segment(0.3f, new Vector2(1.85f, -0.06301107f));
        segments[3] = new Segment(0.2f, new Vector2(3f, -0.06301107f));
        segments[4] = new Segment(0.1f, new Vector2(4f, -0.06301107f));
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

        foreach (Segment segment in segments)
        {
            Collider2D hit = Physics2D.OverlapCircle(segment.GetPosition(), segment.GetRadius(), obstacle_mask);
            if (hit != null)
            {
                // Виштовхування сегмента з колайдера
                Vector2 dir = (segment.GetPosition() - new Vector2(hit.bounds.center.x, hit.bounds.center.y)).normalized;
                segment.SetPosition(hit.ClosestPoint(segment.GetPosition()) + dir * segment.GetRadius());
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Додаємо новий сегмент
            AddSegmentAfterSegment(segments[segments.Count() - 1], 0.1f);

            // Тепер змінюємо радіуси останніх 5-ти для плавного хвоста
            segments[segments.Count() - 5].SetRadius(0.45f);
            segments[segments.Count() - 4].SetRadius(0.4f);
            segments[segments.Count() - 3].SetRadius(0.3f);
            segments[segments.Count() - 2].SetRadius(0.2f);
            segments[segments.Count() - 1].SetRadius(0.1f);
        }
        if (Input.GetKeyDown(KeyCode.Tab)&& segments.Count() > 5)
        {
            RemoveSegment(segments[segments.Count() - 1]);
            //Зміна радіусів для гарного хвоста
            segments[segments.Count() - 1].SetRadius(0.1f);
            segments[segments.Count() - 2].SetRadius(0.2f);
            segments[segments.Count() - 3].SetRadius(0.3f);
            segments[segments.Count() - 4].SetRadius(0.4f);
            segments[segments.Count() - 5].SetRadius(0.45f);
        }
        renderSnake.DrawSnakeMesh(segments);

        RaycastHit2D apple_hit = segments[0].CastForwardCircle(0.5f, apple_mask);

        
        if (apple_hit.collider != null)
        {
            GameObject hit_obj = apple_hit.collider.gameObject;
            Destroy(hit_obj);
            // Додаємо новий сегмент
            AddSegmentAfterSegment(segments[segments.Count() - 1], 0.1f);

            // Тепер змінюємо радіуси останніх 5-ти для плавного хвоста
            segments[segments.Count() - 5].SetRadius(0.45f);
            segments[segments.Count() - 4].SetRadius(0.4f);
            segments[segments.Count() - 3].SetRadius(0.3f);
            segments[segments.Count() - 2].SetRadius(0.2f);
            segments[segments.Count() - 1].SetRadius(0.1f);
        }
        for(int i = 1; i < segments.Count(); i++)
        {
            if (segments[0].CheckIfInsideOtherSegment(segments[i]))
            {
                Debug.Log("Hit!");
                Time.timeScale = 0f;
            }
        }
    }
}
