using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    [Header("Set in Inspector: Enemy_2")]
    // Определяют, насколько ярко будет выражен синусоидальный характер движения
    public float sinEccentricity = 0.6f;
    public float lifeTime = 10;
    // Enemy_2 использует линейную интерполяцию между двумя точками, изменяя результат по синусоиде
    [Header("Set Dynamically :Enemy_2")]
    public Vector3 p0;
    public Vector3 p1;
    public float birthTime;

    void Start()
    {
        //Выбрать случайную точку на правой границе экрана
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        // Случайно поменять начальную и конечную точку местами
        if(Random.value > 0.5f)
        {
            // Измение знака .х каждой точки переносит ее на другой край экрана
            p0.x *= -1;
            p1.x *= -1;
        }
        // Записать в birthTime текущее время
        birthTime = Time.time;

    }
    public override void Move()
    {
        // Кривые Безье вычисляются на основе значение и между 0 и 1
        float u = (Time.time - birthTime) / lifeTime;

        // Если u> 1, значит, корабль существует дольше, чем lifeTime
        if(u>1)
        {
            // Этот экзамплер Enemy_2 завершил свой  жизненный цикл 
            Destroy(this.gameObject);
            return;
       }
        //Скорректировать u добавлением значения кривой, изменяющейся по синусоиде
        u = u + sinEccentricity * (Mathf.Sin(u * Mathf.PI * 2));
        // Интерполировать местоположение между двумя точками 
        pos = (1 - u) * p0 + u * p1;
    }
}
