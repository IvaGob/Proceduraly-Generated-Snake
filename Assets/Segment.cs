using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Segment
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private Vector2 position;
    [SerializeField]
    private Vector2 direction;

    public Segment(float i_radius,Vector2 i_position)
    {
        this.radius = i_radius;
        this.position = i_position;
    }

    //
    public Vector2 GetPosition()
    {
        return position; 
    }
    public void SetPosition(Vector2 pos)
    {
        this.position = pos;
    }
    public float GetRadius()
    {
        return radius;
    }
    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
    public Vector2 GetDirection()
    {
        return direction;
    }
    public void SetDirection(Vector2 direction)
    {
        this.direction = direction;
    }
    public Vector2 GetPointOnCircleAtAngle(float angleOffsetDegrees)
    {
        Vector2 center = position; // ����� ����
        Vector2 dirNorm = direction.normalized;

        // ��� �������� � �������
        float baseAngle = Mathf.Atan2(dirNorm.y, dirNorm.x);

        // ������ ������� (������������ ������� � ������)
        float finalAngle = baseAngle + angleOffsetDegrees * Mathf.Deg2Rad;

        // ��������� ����� �� ���
        Vector2 point = center + new Vector2(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle)) * radius;

        return point;
    }

    public Vector2 GetPointOnCircleAtAngle(float angleOffsetDegrees,float distance)
    {
        Vector2 center = position; // ����� ����
        Vector2 dirNorm = direction.normalized;

        // ��� �������� � �������
        float baseAngle = Mathf.Atan2(dirNorm.y, dirNorm.x);

        // ������ ������� (������������ ������� � ������)
        float finalAngle = baseAngle + angleOffsetDegrees * Mathf.Deg2Rad;

        // ��������� ����� �� ���
        Vector2 point = center + new Vector2(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle)) * distance;

        return point;
    }
    public Vector2[] GetLeftAndRightPointsOnSphere()
    {
        Vector2 center = position; // ����� ����
        Vector2 dirNorm = direction.normalized;

        // ������, ���������������� �� �������� (������ � ��������)
        Vector2 perp = new Vector2(-dirNorm.y, dirNorm.x);

        Vector2 leftPoint = center + perp * radius;
        Vector2 rightPoint = center - perp * radius;

        return new Vector2[] { leftPoint, rightPoint };
    }
    public Vector2 GetBackPoint()
    {
        Vector2 center = this.GetPosition(); // ����� ����
        Vector2 dirNorm = this.GetDirection().normalized;

        // ������, ���������������� �� �������� (������ � ��������)
        Vector2 perp = new Vector2(dirNorm.x, dirNorm.y);
        Vector2 backPoint = center - perp * this.GetRadius();
        return backPoint;
    }
}
