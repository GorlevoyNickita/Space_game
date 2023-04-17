using System.Collections;// Необходимо для доступа к массивам и другим коллекциям
using System.Collections.Generic;//Необходимо для доступа к спискам и словарям
using UnityEngine;// Необходимо для доступа к Unity

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;// скоорость м/с
    public float fireRate = 0.3f;// секунд между выстрелами (не используетьс)
    public float health = 10;
    public  int score = 100;// Очки за уничтожение этого корабля
    public float showDamageDuration = 0.1f; //Длительность эффекта попадания в секундах
    public float powerUpDropChance = 1f; //Вероятность сбросить бонус

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;// Все материалы игрового объекта и его потомков
    public bool ShowingDamage = false;
    public float damageDoneTime;// Время прекращения отображения эффекта
    public bool notifiedofDestruction = false;//Будет использовано позже
    protected BoundsCheck bndCheck;


    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        // Получить матаериалы и цвет этого игрового обьекта и его потомков 
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int  i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }


    //Это свойстово: метод, действующий как поле
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();
        if(ShowingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }
        if (bndCheck != null && bndCheck.offDown)
        {
            // We're off the bottom, so destroy this GameObject
            Destroy(gameObject);
        }
    }
    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGo = coll.gameObject;
        switch(otherGo.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGo.GetComponent<Projectile>();
                //Если вражеский корабль за границами экрана, не наносить ему повреждений.
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGo);
                    break;
                }
                // Поразить вражеский корабль
                ShowDamage();
                //Получить разрушающую из WEAP_DICT в классе Main.
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if(health <= 0)
                {
                    //Сообшить объеку -одиночке Main об уничтожении
                    if (!notifiedofDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedofDestruction = true;
                    // Уничтожить этот вражеский корабль
                    Destroy(this.gameObject);
                }
                Destroy(otherGo);
                break;
            default:
                print("Enemy hit by non - ProjectileHero: " + otherGo.name);
                break;
        }
    }
    void ShowDamage()
    {
        foreach(Material m in materials)
        {
            m.color = Color.red;
        }
        ShowingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }
    void UnShowDamage()
    {
        for(int i =0; i<materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        ShowingDamage = false;
    }
}
