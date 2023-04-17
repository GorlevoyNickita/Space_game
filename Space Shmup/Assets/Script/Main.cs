using System.Collections;// Необходимо для доступа к массивам и другим коллекциям
using System.Collections.Generic;// Необходмо для доступа к спискам и словарям
using UnityEngine;// Необходимо для доступа к Unity
using UnityEngine.SceneManagement;//  для загрузки и перезагрузки

public class Main : MonoBehaviour
{
    static public Main S; // Обьект-одиночка Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;
    [Header("Set in Ispector")]
    public GameObject[] prefabEnemies;//Массив шаблонов Enemmy
    public float enemySpawnPerSecond = 0.5f;//Вражеских кораблей в секундду

    public float enemyDefaultPadding = 1.5f;// Отступ дя позиционирования

    public float enemySpawnPerSeccond = 3.5f;// Вражеских кораблей в секунду
    public float enenmyDefaultPadding = 1.5f;//Отступ для позиционирования
    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {WeaponType.blaster, WeaponType.blaster,WeaponType.spread, WeaponType.shield };

    private BoundsCheck bndCheck;


    public void ShipDestroyed(Enemy e)
    {
        //Сгенерировать бонус с заданной вероятностью
        if(Random.value <= e.powerUpDropChance)
        {
            //Выбрать тип бонуса
            //Выбрать один из элементов в powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];
            //Создать Экзампляр PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            // Установить соотвеющий тип WeaponType
            pu.SetType(puType);
        }
    }
    void Awake()
    {
        S = this;
        //  Записать в bndCheck ссылку на компонент Boundscheck этого игрового обьекта
        bndCheck = GetComponent<BoundsCheck>();
        //Вызывать SpawnEnemy() один раз (в 2 секунды при значениях по умолчанию)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        // Словарь с ключами типа WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }

    }

    public void SpawnEnemy()
    {
        // Выбрать случайным шаблон Enemy для создания
        int ndx = Random.Range(0,prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
        // Разместить вражеский корабль над экраном в случайном позиции
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }
        // Установить начальные кординты созданого вражеского корабля 
        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyPadding;
        float xMax = bndCheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyPadding;
        go.transform.position = pos;
        // снова вызвать SpawnEnemy()
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        //Вызвать иетод Restart() через delay секунд
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        // Перезагрузить _Scene_0, чтобы перезапустить игру
        SceneManager.LoadScene("Scene_0");
    }
    /// <summary>
    /// Статическая функция, возвращающая WeaponDefinition из  статического защищенного поля WEAP_DICT класса Main
    /// </summary>
    /// <param name="wt">Тип оржия WeaponType, для которого требуется получить WeaponDefinition</param>
    /// <returns>Экземпляр WeaponDefinition или, если нет такого опредеоения для указаного WeaponType, возвращает новый экзкмпляр WeaponDefinition
    ///  с типом none.</returns>
    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        //Проверить наличие указоного ключа в словаре
        //Попытка извлечь значение по отсутствующему ключу вызовет ошибку, поэтому следуйщая инструкция играет важную роль.
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        // Следующая инструкция возвращает новый экземпляр WeaponDefinition с типом оружия WeaponType.none, что означает неудачную попытку найти требуемое опредление WeaponDefinition
        return (new WeaponDefinition());
    }
}
