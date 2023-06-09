using System.Collections;// ���������� ��� ������� � �������� � ������ ����������
using System.Collections.Generic;// ��������� ��� ������� � ������� � ��������
using UnityEngine;// ���������� ��� ������� � Unity
using UnityEngine.SceneManagement;//  ��� �������� � ������������

public class Main : MonoBehaviour
{
    static public Main S; // ������-�������� Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;
    [Header("Set in Ispector")]
    public GameObject[] prefabEnemies;//������ �������� Enemmy
    public float enemySpawnPerSecond = 0.5f;//��������� �������� � ��������

    public float enemyDefaultPadding = 1.5f;// ������ �� ����������������

    public float enemySpawnPerSeccond = 3.5f;// ��������� �������� � �������
    public float enenmyDefaultPadding = 1.5f;//������ ��� ����������������
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {WeaponType.blaster, WeaponType.blaster,WeaponType.spread, WeaponType.shield };

    private BoundsCheck bndCheck;


    public void ShipDestroyed(Enemy e)
    {
        //������������� ����� � �������� ������������
        if(Random.value <= e.powerUpDropChance)
        {
            //������� ��� ������
            //������� ���� �� ��������� � powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];
            //������� ��������� PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // ���������� ���������� ��� WeaponType
            pu.SetType(puType);
        }
    }
    void Awake()
    {
        S = this;
        //  �������� � bndCheck ������ �� ��������� Boundscheck ����� �������� �������
        bndCheck = GetComponent<BoundsCheck>();
        //�������� SpawnEnemy() ���� ��� (� 2 ������� ��� ��������� �� ���������)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        // ������� � ������� ���� WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }

    }

    public void SpawnEnemy()
    {
        // ������� ��������� ������ Enemy ��� ��������
        int ndx = Random.Range(0,prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        // ���������� ��������� ������� ��� ������� � ��������� �������
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        // ���������� ��������� �������� ��������� ���������� ������� 
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;
        // ����� ������� SpawnEnemy()
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        //������� ����� Restart() ����� delay ������
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        // ������������� _Scene_0, ����� ������������� ����
        SceneManager.LoadScene("Scene_0");
    }
    /// <summary>
    /// ����������� �������, ������������ WeaponDefinition ��  ������������ ����������� ���� WEAP_DICT ������ Main
    /// </summary>
    /// <param name="wt">��� ����� WeaponType, ��� �������� ��������� �������� WeaponDefinition</param>
    /// <returns>��������� WeaponDefinition ���, ���� ��� ������ ����������� ��� ��������� WeaponType, ���������� ����� ��������� WeaponDefinition
    ///  � ����� none.</returns>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        //��������� ������� ��������� ����� � �������
        //������� ������� �������� �� �������������� ����� ������� ������, ������� ��������� ���������� ������ ������ ����.
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        // ��������� ���������� ���������� ����� ��������� WeaponDefinition � ����� ������ WeaponType.none, ��� �������� ��������� ������� ����� ��������� ���������� WeaponDefinition
        return (new WeaponDefinition());
    }
}
