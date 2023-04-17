using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1z")]
    // ����� ������ ������� ����� ���������
    public float waveFrequency = 2;
    // ������  ��������� � ������
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0; // ��������� �������� ���������� �
    private Vector3 posy;
    private float birthTime;
   //private BoundsCheck bndCheck;

    // �����  Start ������ �������� ��� ����� �����, ������ ��� �� ������������ ������������ Enemy
    void Start()
    {
        // ���������� ��������� ���������� � ������ Enemy_1 
        x0 = pos.x;
        birthTime = Time.time;
    }
 
    // �������������� ������� Move ����������� Enemy
    public override void Move()
    {
        // ��� ��� pos - ��� ���������, ������ �������� �������� pos.x
        Vector3 tempPos = pos;
        // �������� theta ���������� � �������� �������;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        // base.Move() ���������� �������� ����, ����� ��� Y 
        base.Move();

       // print(bndCheck.isOnScreen);
    }
}
