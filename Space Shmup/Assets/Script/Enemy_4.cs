using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Part --  еще один сериализуемый класс подобно WeaponDefinition предназначенный для хранений данных
/// </summary>
/*[System.Serializable]
public class Part
{
    //Значение этих трех полей должні определяться в инспекторе
    public string name;// имя этой части
    public float health;//Степень стойкости этой части
    public string[] protecedBy;//Другие части, защищающие эту

    //Эти два поля инициализируются автоматически в Start().
    //Кэширование, как здесь, ускоряют получение необходимых данных
    [HideInInspector]// Не позволяют следующему полю появиться в инспекторе
    public GameObject go;// Игровой объект этой части 
    [HideInInspector]
    public Material mat;// Материал для отображения повреждений
}
*/
/// <summary>
/// Enemy_4 создаеться за верхней границей, выбирает случайную точку на экране и перемещается к ней. Добравшись до места, выбирает другую случайную точку и продолжает двигаться, поа игрок не уничтожит его.
/// </summary>
public class Enemy_4 : Enemy
{
    [Header("Set in Inspector:Enemy_4")]
    //public Part[] parts;// Массив частей, составляющих корабль
    private Vector3 p0, p1;// две точки для интеполяции
    private float timeStart;// Время создания этого корабля 
    private float duration = 4;// Продолжительность перемещения

    void Start()
    {
        // Начальная позиция уже выбрана в Main.SpawnEnemy(), поэтому запишем ее как начальное значение в p0 и p1
        p0 = p1 = pos;
        InitMovement();
        //Записать в кэш игровой объект и материал каждой части в parts
      /*  Transform t;
        foreach(Part prt in parts)
        {
            t = transform.Find(prt.name);
            if(t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
      */
    }
    void InitMovement()
    {
        p0 = p1;//переписать p1 в p0
        // Выбрать новую точку p1 на экране
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        p1.x = Random.Range(-widMinRad, widMinRad);
        p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        // Сбросить время
        timeStart = Time.time;
    }

    public override void Move()
    {
        // Этот метод переопредиляет Enemy.Move()  и реализует линейную интерполяцию
        float u = (Time.time - timeStart) / duration;

        if (u >= 1)
        {
            InitMovement();
            u = 0;
        }
        u = 1 - Mathf.Pow(1 - u, 2);// Применить плавное замедление
        pos = (1 - u) * p0 + u * p1;//Простая линейная интеполяция
    }
/*і
    //Эти две функции ввыполняют поиск части в массиве parts  по имени или по ссылке на игровой объект
    Part FindPart(string n) // a
    {
        foreach(Part prt in parts)
        {
            if(prt.name == n)
            {
                return (prt);
            }
        }
        return (null);
    }
    Part FindPart(GameObject go)  // b
    {
        foreach(Part prt in parts)
        {
            if(prt.go == go)
            {
                return (prt);
            }
        }
        return (null);
    }
    // Эти функции возвращают true, если данная часть уничтожена 
    bool Destroyed(GameObject go)// с
    {
        return (Destroyed(FindPart(go)));
    }

    bool Destroyed(string n)
    {
        return (Destroyed(FindPart(n)));
    }

   bool Destroyed(Part prt)
    {
        if (prt == null) //  Если ссылка на часть не была передана
        {
            return (true);// Вернуть true (то есть: да, была уничтожена)
        }
        // Вернуть результат сравнения:prt.health <= 0
        // Если prt.health <= 0, вернуть true(да, была унчитожена)
        return (prt.health <= 0);
    }
    // Окрашивает в красный только одну часть, а не весь корабль.
    void ShowLocalizedDamage(Material m) // d
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        ShowingDamage = true;
    }

    //Переодпределяет метод OnCollisionEnter из сценария Enemy.cs.
    void OnCollisionEnter(Collision coll) // e
    {
        GameObject other = coll.gameObject.gameObject;
        switch (other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                // Если корабль за границами экрана, не повреждать его. 
                if (!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                // Поразить вражеский корабль
                GameObject goHit = coll.contacts[0].thisCollider.gameObject; //f
                Part prtHit = FindPart(goHit);
                if (prtHit == null) // if prthit wasnt found...// g
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                // Проверить, защищена ли еще эта часть корабля
                if (prtHit.protecedBy != null) //h
                {
                    foreach (string s in prtHit.protecedBy)
                        // Если хотя бы одна из защищающих частей еще не разрушена ...
                    {
                        if(!Destroyed(s))
                        {
                            //... НЕ наносить повреждений этой части
                            Destroy(other);// Уничтожить снаряд ProjectileHero
                            return;// выйти, не повреждая Enemy_4
                        }
                    }
                }
                // Эта часть не защищена, нанести ей повреждение
                // Получить разрушающею силу из Projectile.type и Main.WEAP_DICT
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                // Показать эффект попадания в часть
                ShowLocalizedDamage(prtHit.mat);
                if(prtHit.health <= 0) // i
                    // Виесто разрушения всего корабля
                    //деактивировать уничтоженную часть 
                {
                    prtHit.go.SetActive(false);
                }
                //Проверить, был ли корабль полностью разрушен
                bool allDestroyed = true;// Предположить, что разрушен
                foreach (Part prt in parts )
                {
                    if (!Destroyed(prt)) // если какая - то часть еще существует...
                    {
                        allDestroyed = false; // записать false в allDestroyed
                        break; // и прервать цикл foreach
                    }
                }
                if (allDestroyed) // Если корабль разрушен полностью...// j
                {
                    //... уведомить объект- одиночку Main, что этот корабль разрушен
                    Main.S.ShipDestroyed(this);
                    //Уничтожить этот объект Enemy
                    Destroy(this.gameObject);
                }
                Destroy(other);// Уничтожить снаряд ProjectileHero
                break;
        }
    }
    */
}
// заметки 677 