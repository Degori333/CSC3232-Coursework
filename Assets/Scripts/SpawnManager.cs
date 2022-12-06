using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject whirlpoolPrefab;
    public GameObject shipLvl1Prefab;
    public GameObject shipLvl2Prefab;
    public GameObject shipLvl3Prefab;
    public GameObject shop;

    public float horizontalSpawnDistnace = 50f;
    public float verticalSpawnDistnace = 50f;

    public float offset = 0f;

    public float horizontalBorder = 1000f;
    public float verticalBorder = 1000f;

    [SerializeField] private int enemiesPerWave = 1;
    [SerializeField] private float delay = 17f;

    private List<EnemyController> enemies = new();

    public int EnemiesPerWave
    {
        get
        {
            return enemiesPerWave;
        }
        set
        {
            if (value >= 4)
            {
                enemiesPerWave = 4;
            }
            else enemiesPerWave = value;
        }
    }
    [SerializeField] private int enemiesLeftToSpawn = 1;
    [SerializeField] private int enemiesAlive;
    [SerializeField] bool shopOpen = false;
    public int EnemiesAlive
    {
        get
        {
            return enemiesAlive;
        }
        set
        {
            if (value == 0 && !shopOpen)
            {
                enemies.Clear();
                OpenShop();
            }
            else enemiesAlive = value;
        }
    }

    private void Start()
    {
        StartWave();
        StartCoroutine(CreateWhirlpool());
    }
    private void Update()
    {
        int alive = 0;
        foreach (EnemyController enemy in enemies)
        {
            
            if (!enemy.isDead)
            {
                alive++;
            }
            else
            {
                Destroy(enemy.gameObject);
            }
        }
        EnemiesAlive = alive;
    }

    public void OpenShop()
    {
        shopOpen = true;
        enemiesPerWave++;
        GameManager.gameManager.ToggleActive(shop);
        GameManager.gameManager.PauseGame();
    }

    public void StartWave()
    {
        shopOpen = false;
        for (enemiesLeftToSpawn = enemiesPerWave; enemiesLeftToSpawn > 0; enemiesLeftToSpawn--)
        {
            Spawn(ChooseRandomShip(), PickRandomSpawnPointOnEdge());
        }
        enemiesAlive = enemiesPerWave;
    }

    GameObject ChooseRandomShip()
    {
        if (Random.value < 0.8f)
        {
            return shipLvl1Prefab;
        }
        else if (Random.value < 0.99f)
        {
            return shipLvl2Prefab;
        }
        else return shipLvl3Prefab;
    }

    IEnumerator CreateWhirlpool()
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            Spawn(whirlpoolPrefab, PickRandomSpawnPointInside());
        }
    }

    void Spawn(GameObject prefab, Vector3 location)
    {
        GameObject newShip = Instantiate(prefab, location, Quaternion.identity);
        if (prefab != whirlpoolPrefab)
        {
            newShip.AddComponent<EnemyController>();
            EnemyController controller = newShip.GetComponent<EnemyController>();
            controller.unitData = (UnitData)Resources.Load(prefab.name);
            controller.Restart();
            enemies.Add(controller);
        }
    }

    Vector3 PickRandomSpawnPointInside()
    {
        Vector3 location;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        do
        {
            float x = Random.Range(-horizontalSpawnDistnace, horizontalSpawnDistnace);
            float z = Random.Range(-verticalSpawnDistnace, verticalSpawnDistnace);

            location = new Vector3(player.transform.position.x + x, 0, player.transform.position.z + z);

        }
        while (!SampleLocation(location));
        return location;
    }

    Vector3 PickRandomSpawnPointOnEdge()
    {
        Vector3 location;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        do
        {
            float x;
            float z;
            if (Random.value < 0.25f)
            {
                x = -horizontalSpawnDistnace - offset;
                z = Random.Range(-verticalSpawnDistnace - offset, verticalSpawnDistnace + offset);
            }
            else if (Random.value < 0.5f)
            {
                x = horizontalSpawnDistnace + offset;
                z = Random.Range(-verticalSpawnDistnace - offset, verticalSpawnDistnace + offset);
            }
            else if (Random.value < 0.75f)
            {
                x = Random.Range(-horizontalSpawnDistnace - offset, horizontalSpawnDistnace + offset);
                z = -verticalSpawnDistnace - offset;
            }
            else
            {
                x = Random.Range(-horizontalSpawnDistnace - offset, horizontalSpawnDistnace + offset);
                z = verticalSpawnDistnace + offset;
            }

            location = new Vector3(player.transform.position.x + x, 0, player.transform.position.z + z);

        }
        while (!SampleLocation(location));
        return location;
    }

    bool SampleLocation(Vector3 location)
    {
        if (location.x > -horizontalBorder && location.x < horizontalBorder && location.z > -verticalBorder && location.z < verticalBorder)
        {
            return true;
        }
        else return false;
            
    }
}
