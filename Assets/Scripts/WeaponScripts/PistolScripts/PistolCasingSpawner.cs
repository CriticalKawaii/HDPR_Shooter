using UnityEngine;

public class PistolCasingSpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    [SerializeField] private GameObject prefab; // Префаб для создания
    [SerializeField] private Transform spawnPoint; // Точка спавна (относительно этого объекта будет направление)
    [SerializeField] private float impulseForce = 10f; // Базовая сила импульса
    [SerializeField] private Vector3 impulseDirection = new Vector3(-1f, 1f, 0f); // Базовое направление относительно spawnPoint
    [SerializeField] private float lifeTime = 60f; // Время жизни в секундах (1 минута = 60)
    
    [Header("Настройки поворота")]
    [SerializeField] private bool matchSpawnPointRotation = true; // Поворачивать префаб как spawnPoint
    [SerializeField] private Vector3 rotationOffset = Vector3.zero; // Доп. поворот относительно spawnPoint
    
    [Header("Настройки рандомизации")]
    [SerializeField] [Range(0f, 100f)] private float forceRandomPercent = 20f; // Рандомизация силы в процентах
    [SerializeField] [Range(0f, 100f)] private float directionRandomPercent = 15f; // Рандомизация направления в процентах

    // Публичный метод для вызова из других скриптов
    private void Spawn()
    {
        if (prefab == null)
        {
            Debug.LogError("Префаб не назначен!");
            return;
        }

        // Определяем точку спавна и объект для относительного направления
        Transform targetTransform = spawnPoint != null ? spawnPoint : transform;
        
        Vector3 spawnPosition = targetTransform.position;
        
        // Базовый поворот от targetTransform
        Quaternion baseRotation = matchSpawnPointRotation ? targetTransform.rotation : Quaternion.identity;
        
        // Применяем rotationOffset и добавляем -90 по X
        Quaternion finalRotation = baseRotation * Quaternion.Euler(rotationOffset) * Quaternion.Euler(-90f, 0f, 0f);

        // Создаем префаб
        GameObject spawnedObject = Instantiate(prefab, spawnPosition, finalRotation);
        
        // Получаем или добавляем Rigidbody для импульса
        Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = spawnedObject.AddComponent<Rigidbody>();
        }
        
        // Рандомизируем направление
        Vector3 randomizedDirection = RandomizeDirection(impulseDirection);
        
        // Преобразуем локальное направление targetTransform в мировое пространство
        Vector3 worldDirection = targetTransform.TransformDirection(randomizedDirection.normalized);
        
        // Рандомизируем силу
        float randomizedForce = RandomizeForce(impulseForce);
        
        // Применяем импульс
        rb.AddForce(worldDirection * randomizedForce, ForceMode.Impulse);
        
        // Уничтожаем через заданное время
        Destroy(spawnedObject, lifeTime);
        
        Debug.Log($"Префаб создан на позиции {spawnPosition}. " +
                  $"Сила: {randomizedForce} (базовая: {impulseForce}), " +
                  $"Направление: {worldDirection}");
    }

    // Перегруженный метод для спавна с задержкой
    public void PistolCasSpawn(float delay)
    {
        Invoke(nameof(Spawn), delay);
    }

    // Метод для спавна с кастомным направлением относительно spawnPoint
   
    private float RandomizeForce(float baseForce)
    {
        float percent = forceRandomPercent / 100f;
        float randomFactor = Random.Range(1f - percent, 1f + percent);
        return baseForce * randomFactor;
    }

    private Vector3 RandomizeDirection(Vector3 baseDirection)
    {
        float percent = directionRandomPercent / 100f;
        
        // Создаем случайное отклонение для каждой оси
        float randomX = Random.Range(1f - percent, 1f + percent);
        float randomY = Random.Range(1f - percent, 1f + percent);
        float randomZ = Random.Range(1f - percent, 1f + percent);
        
        // Применяем отклонение к направлению
        Vector3 randomized = new Vector3(
            baseDirection.x * randomX,
            baseDirection.y * randomY,
            baseDirection.z * randomZ
        );
        
        return randomized.normalized;
    }

    private void OnValidate()
    {
        if (impulseDirection.magnitude > 0)
        {
            impulseDirection = impulseDirection.normalized;
        }
        else
        {
            impulseDirection = new Vector3(-1f, 1f, 0f).normalized;
        }
    }
}