using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1z")]
    // число секунд полного цикла синусоиды
    public float waveFrequency = 2;
    // ширина  синусоиды в метрах
    public float waveWidth = 4;
    public float waveRotY = 45;

    private float x0; // Начальное значение координаты Х
    private Vector3 posy;
    private float birthTime;
   //private BoundsCheck bndCheck;

    // Метод  Start хорошо подходит для наших целей, потому что не используется суперклассом Enemy
    void Start()
    {
        // Установить начальную координату Х объект Enemy_1 
        x0 = pos.x;
        birthTime = Time.time;
    }
 
    // Переопределить функцию Move суперкласса Enemy
    public override void Move()
    {
        // Так как pos - это свойтство, нельзя напрямую изменить pos.x
        Vector3 tempPos = pos;
        // значение theta изменяется с течением времени;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);
        // base.Move() обратывает движение вниз, вдоль оси Y 
        base.Move();

       // print(bndCheck.isOnScreen);
    }
}
