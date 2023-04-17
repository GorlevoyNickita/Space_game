using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in Inspector")]
    // Необычное, но удобное применение Vector2.х хранит минимальное значение дл метода Random.Range, который будет вызываться позже
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f;// Время в секундах существования PowerUp
    public float fadeTime = 4;//  Время в секундах исчезновение PowerUp

    [Header("Set Dynamically")]
    public WeaponType type; // тип бонуса
    public GameObject cube;// Ссылка на вложеный куб
    public TextMesh letter;//  Ссылка на TextMesh
    public Vector3 rotPerSecond;// Скорость вращения
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    void Awake()
    {
        // Получить Ссылку на куб
        cube = transform.Find("Cube").gameObject;
        //Получить ссылки на TextMesh и другие компоненты
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = cube.GetComponent<Renderer>();

        //Выбрать случайную скорость 
        Vector3 vel = Random.onUnitSphere;// Получить случайную скорость XYZ
        //Random.onUnitSphere  возвращает вектор, указывающий на случайную точку, находящуюся на поверхности сферы с радиусом 1 м  и сцентром в начале координат 
        vel.z = 0;// Отобразить vel на плоскость vel.z = 0; // Отобразить vel на плоскость XY
        vel.Normalize();// Нормализация устанавливает длину Vector3 равной 1 м
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;
        // Установить угол поворота этого игрового объекта равным R:[ 0, 0, 0 ]
        transform.rotation = Quaternion.identity;
        //Quaternion.identity; равноцено отстуствию поворота.
        //Выбрать случайную скорость вращения для вложенного куба с использыванием rotMinMax.х и rotMinMax.у
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y), Random.Range(rotMinMax.x,rotMinMax.y),Random.Range(rotMinMax.x,rotMinMax.y));
        birthTime = Time.time;
    }

     void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        // Эффект растворения куба PowerUp с течением времени со сначением по умолчанию бонус существует 10 секунд ,а затем растворяеться в течение 4 секунд 
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        // В течение lifeTime секунд значение и будет <= 0.Затем ого станет положительным и через fadeTime секунд станет больше 1.
        // Если (u >= 1, уничтожить бонус
        if (u >= 1)
        {
            Destroy(this.gameObject);
        }
        return;
        //Использовать u для опредиления альфа-значения куба и буквы
        if (u > 0)
        {
            Color c = cubeRend.material.color;
            c.a = 1f - u;
            cubeRend.material.color = c;
            // Буква тоже должна растворяться, но медленнее
            c = letter.color = c;

        }
        if(!bndCheck.isOnScreen)
        {
            // Если бонус полностью вышел за границу экрана, уничтожить его
            Destroy(gameObject);
        }
    }
    public void SetType(WeaponType wt)
    {
        // Получить WeaponDefinition из Main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        //Установить цвет дочерного куба
        cubeRend.material.color = def.color;
        //letter.color = def.color; // Букву тоже можно окрасить в тот же цвет
        letter.text = def.letter;//Установить отображаемую букву
        type = wt;// В заключение установить фактичесий тип
    }

    public void AbsorbedBy(GameObject target)
    {
        //Это функция вызываеться классом Него, когда игрок подбирает бонус 
        //Можно было бы реализовать эффект поглощения бонуса, уменьшая его размеры в течение нескольких кадров, но пока просто унчитожим  this.gameObject
        Destroy(this.gameObject);
    }
}
