using System.Collections;// ���������� ��� ������� � �������� � ������ ����������
using System.Collections.Generic;//���������� ��� ������� � ������� � ��������
using UnityEngine;// ���������� ��� ������� � Unity

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 10f;// ��������� �/�
    public float fireRate = 0.3f;// ������ ����� ���������� (�� ������������)
    public float health = 10;
    public  int score = 100;// ���� �� ����������� ����� �������
    public float showDamageDuration = 0.1f; //������������ ������� ��������� � ��������
    public float powerUpDropChance = 1f; //����������� �������� �����

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials;// ��� ��������� �������� ������� � ��� ��������
    public bool ShowingDamage = false;
    public float damageDoneTime;// ����� ����������� ����������� �������
    public bool notifiedofDestruction = false;//����� ������������ �����
    protected BoundsCheck bndCheck;


    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        // �������� ���������� � ���� ����� �������� ������� � ��� �������� 
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int  i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }


    //��� ���������: �����, ����������� ��� ����
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
                //���� ��������� ������� �� ��������� ������, �� �������� ��� �����������.
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGo);
                    break;
                }
                // �������� ��������� �������
                ShowDamage();
                //�������� ����������� �� WEAP_DICT � ������ Main.
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if(health <= 0)
                {
                    //�������� ������ -�������� Main �� �����������
                    if (!notifiedofDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedofDestruction = true;
                    // ���������� ���� ��������� �������
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
