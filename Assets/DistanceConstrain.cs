using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class DistanceConstrain : MonoBehaviour
{
    [SerializeField]
    private Segment[] points;
    public float target_distance;
    public float min_angle;
    public void SetPoints(Segment[] segments)
    {
        points = segments; 
    }
    public Segment[] getPoints()
    {
        return points;
    }
    public void ConnectTwoObjects(Segment a, Segment b,float targetDistance)
    {
        Vector2 aPos = a.GetPosition();
        Vector2 bPos = b.GetPosition();
        Vector2 distance = aPos - bPos;
        float normalDistance = distance.magnitude;
        //Debug.Log(normalDistance);
        distance.x /= normalDistance;
        distance.y /= normalDistance;
        b.SetPosition(new Vector2(b.GetPosition().x + distance.x * (normalDistance - targetDistance), b.GetPosition().y+ distance.y * (normalDistance - targetDistance)));
        b.SetDirection(a.GetPosition() - b.GetPosition());
    }
    public void AngleConstraint(ref Vector2 a, Vector2 b, Vector2 c, float min_angle)
    {
        Vector2 v1 = a - b; // вектор від b до a
        Vector2 v2 = c - b; // вектор від b до c
        Debug.DrawRay(b, v1);
        Debug.DrawRay(b, v2);
        // Якщо один з векторів нульовий — нічого робити
        if (v1.sqrMagnitude < Mathf.Epsilon || v2.sqrMagnitude < Mathf.Epsilon)
            return;

        float angle = Vector2.Angle(v1, v2); // завжди невід'ємний (0..180)
        if (angle < min_angle)
        {
            float d_angle = min_angle - angle; // скільки треба повернути

            // Підписаний кут від v2 до v1 (від -180 до 180)
            float signed = Vector2.SignedAngle(v2, v1);

            // Якщо підписаний кут ≈ 0 (точно колінеарні), оберемо напрямок за замовчуванням
            float sign = Mathf.Approximately(signed, 0f) ? 1f : Mathf.Sign(signed);

            // Поворот вектора v1 на d_angle у напрямку sign
            float rotateBy = sign * d_angle;
            Vector3 rotated3 = Quaternion.AngleAxis(rotateBy, Vector3.forward) * (Vector3)v1;
            Vector2 rotated = (Vector2)rotated3;

            // rotated вже зберігає початкову довжину v1 (робота Quaternion)
            a = b + rotated;
        }
    }

    public float CalculateAngleBetweenTwoPoints(Vector2 a,Vector2 b, Vector2 c)
    {
        Vector2 v1 = a - b;
        Debug.DrawRay(b, v1);
        Vector2 v2 = c - b;
        Debug.DrawRay(b, v2);
        // Кут між сегментами
        return Vector2.Angle(v1, v2);
    }
    // Update is called once per frame
    void Update()
    {
        //З'єднуємо
        for (int i = 0; i < points.Length - 1; i++)
        {
            ConnectTwoObjects(points[i], points[i+1], points[i].GetRadius());
        }
        //Повертаємо
        for (int i = points.Length - 1; i >= 2; i--)
        {
            Vector2 pos = points[i].GetPosition();
            AngleConstraint(ref pos, points[i - 1].GetPosition(), points[i - 2].GetPosition(), min_angle);
            points[i].SetPosition(pos);
        }
    }

}
