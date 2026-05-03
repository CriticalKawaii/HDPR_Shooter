using UnityEngine;

public class TargetSpawning : MonoBehaviour
{
    [Header("Spawn Lines")]
    [SerializeField] private float[] spawnX = new float[5];

    [Header("Local Spawn Position")]
    [SerializeField] private float spawnY;
    [SerializeField] private float spawnZ;

    [Header("Settings")]
    [SerializeField] private int targetNumber = 20;
    [SerializeField] private float spawnDelay = 3f;

    [Header("References")]
    [SerializeField] private GameObject torsoPrefab;
    [SerializeField] private GameObject parentObject;
    [SerializeField] ButtonStart BTS;

    private int spawnedNumber = 0;
    private float timer = 0f;
    private bool isRunning = false;

    public void Starter()
    {
        spawnedNumber = 0;
        timer = 0f;
        isRunning = true;
    }

    private void Update()
    {
        if (!isRunning)
            return;

        if (spawnedNumber >= targetNumber)
        {
            isRunning = false;
            BTS.ResetButton();
            return;
        }

        timer += Time.deltaTime;

        if (timer >= spawnDelay)
        {
            timer = 0f;
            SpawnRandomly();
        }
    }

    private void SpawnRandomly()
    {
        if (spawnX.Length < 5)
        {
            Debug.LogWarning("spawnX должен содержать 5 координат!");
            return;
        }

        int randomLine = Random.Range(0, spawnX.Length);

        Vector3 localSpawnPos = new Vector3(
            spawnX[randomLine],
            spawnY,
            spawnZ
        );

        GameObject spawnedTarget = Instantiate(
            torsoPrefab,
            parentObject.transform
        );

        spawnedTarget.transform.localPosition = localSpawnPos; 
        spawnedTarget.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        spawnedNumber++;

        Debug.Log("Spawned target #" + spawnedNumber +
                  " on line " + randomLine);
    }
}