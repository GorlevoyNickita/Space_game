using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ��� ������������ ���� ��������� ����� ������
/// ����� �������� ��� "shield", ����� ���� ����������� ���������������� ������.
/// ������������� [HP] ���� �������� ��������, �� ������������ � ���� �����
/// </summary>

public enum WeaponType
{
    none,// �� ���������/ ��� ������
    blaster,// ������� �������
    spread,// ������� �����, ���������� ����������� ���������
    phaser,// [HP] ������ �����
    missile,// [HP] ��������������� ������
    laser,// [HP] ������� ����������� ��� �������������� �����������
    shield// ����������� shieldLevel
}
/// <summary>
/// ����� WeaponDefinition ��������� ������������ �������� ���������� ���� ������ � ����������. ��� ����� ����� Main
/// ����� ������� ������ ��������� ���� WeaponDefinition
/// </summary>

[System.Serializable]
public class WeaponDefinition
{
    public WeaponType type = WeaponType.none;
    public string letter;// ����� �� ������, ���������� �����

    public Color color = Color.white;//���� ������ ������ � ������ ������
    public GameObject projectilePrefab;// ������ ��������
    public Color projectileColor = Color.white;
    public float damageOnHit = 0;//�������������� ��������
    public float continuousDamage = 0;// ������� ���������� � ������� (��� Laser)

    public float delayBetweenShots = 0;
    public float velocity = 20;// �������� ������ ��������
        }
public class Weapon : MonoBehaviour
{
    static public Transform PROJECTILE_ANCHOR;
    [Header("Set Dynamically")]
    [SerializeField]
    private WeaponType _type = WeaponType.none;
    public WeaponDefinition def;
    public GameObject collar;
    public float lastShotTime; // ����� ���������� ��������
    private Renderer collarRend;

    void Start()
    {

        collar = transform.Find("Color").gameObject;
        collarRend = collar.GetComponent<Renderer>();
        //������� SetType(), ����� �������� ��� ������ �� ��������� WeaponType.none
        SetType(_type); // a
        // ����������� ������� ����� �������� ��� ���� ��������
        if (PROJECTILE_ANCHOR == null)//b
        {
            GameObject go = new GameObject("_ProjectileAnchor");
            PROJECTILE_ANCHOR = go.transform;
        }
        // ����� fireDelegate � �������� ������� �������
        GameObject rootGO = transform.root.gameObject;
        if(rootGO.GetComponent<Hero>() != null)//c
        {
            rootGO.GetComponent<Hero>().fireDelegate += Fire;//d
        }
    }
    public WeaponType type
    {
       get { return (_type); }
       set { SetType(value); }
    }

    public void SetType (WeaponType wt)
    {
        _type = wt;
        if(type == WeaponType.none) //e
        {
            this.gameObject.SetActive(false);
            return;
        }
        else
        {
            this.gameObject.SetActive(true);
        }
        def = Main.GetWeaponDefinition(_type); // f
        collarRend.material.color = def.color;
        lastShotTime = 0;// ����� ����� ���������  _type ����� ���������� // g
    }

    public void Fire()
    {
        // ���� this.gameobject ���������, ����� 
        if (!gameObject.activeInHierarchy) return; //h
        // ���� ����� ���������� ������ ������������ ����� �������, �����
        if(Time.time - lastShotTime < def.delayBetweenShots) // i
        {
            return;
        }
        Projectile p;
        Vector3 vel = Vector3.up * def.velocity; // j
        if (transform.up.y < 0)
        {
            vel.y = -vel.y;
        }
        switch (type) //k
        {
            case WeaponType.blaster:
                p = MakeProjectile();// ������, ������� ����� 
                p.rigid.velocity = vel;
                break;
            case WeaponType.spread: //l
                p = MakeProjectile();
                p.rigid.velocity = vel;
                p = MakeProjectile();//������, ������� ������
                p.transform.rotation = Quaternion.AngleAxis(10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                p = MakeProjectile();//������, ������� �����
                p.transform.rotation = Quaternion.AngleAxis(-10, Vector3.back);
                p.rigid.velocity = p.transform.rotation * vel;
                break;
        }
    }
    public Projectile MakeProjectile() //m
    {
        GameObject go = Instantiate<GameObject>(def.projectilePrefab);
        if( transform.parent.gameObject.tag == "Hero") //n
        {
            go.tag = "ProjectileHero";
            go.layer = LayerMask.NameToLayer("ProjectileHero");
        }
        else
        {
            go.tag = "ProjectileEnemy";
            go.layer = LayerMask.NameToLayer("ProjectileEnemy");
        }
        go.transform.position = collar.transform.position;
        go.transform.SetParent(PROJECTILE_ANCHOR, true); //o
        Projectile p = go.GetComponent<Projectile>();
        p.type = type;
        lastShotTime = Time.time; //p
        return (p);
    }
}
// 648 ��� �������