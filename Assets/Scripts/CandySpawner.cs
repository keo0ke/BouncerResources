using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandySpawner : MonoBehaviour
{
    [SerializeField] private GameObject _candyPrefab;
    [SerializeField] private Material[] _candyColors; // Красный, синий, зеленый
    [SerializeField] private float _spawnRange = 9f;
    [SerializeField] private float _minDistanceFromObjects = 1.5f; // Минимальное расстояние от других объектов
    [SerializeField] private int _maxAttempts = 30; // Максимальное количество попыток найти свободное место

    private List<Vector3> _occupiedPositions = new List<Vector3>(); // Список занятых позиций

    private void Start()
    {
        // Находим все существующие объекты на сцене
        UpdateOccupiedPositions();
        SpawnCandy();
    }

    public void SpawnCandy()
    {
        if (_candyPrefab == null)
        {
            Debug.LogError("[CandySpawner] Префаб конфеты не назначен!");
            return;
        }

        // Обновляем список занятых позиций перед спавном
        UpdateOccupiedPositions();

        // Ищем свободную позицию
        Vector3 randomPosition = GetFreePosition();

        // Если не нашли свободное место после всех попыток, используем случайную позицию
        if (randomPosition == Vector3.zero)
        {
            randomPosition = new Vector3(
                Random.Range(-_spawnRange, _spawnRange),
                1f,
                Random.Range(-_spawnRange, _spawnRange)
            );
            Debug.LogWarning($"[CandySpawner] Не найдено свободное место, используем случайную позицию {randomPosition}");
        }

        GameObject candy = Instantiate(_candyPrefab, randomPosition, Quaternion.identity);

        // Добавляем и настраиваем контроллер
        CandyController controller = candy.GetComponent<CandyController>();
        if (controller == null)
        {
            controller = candy.AddComponent<CandyController>();
        }

        // Устанавливаем материалы
        var colorsField = controller.GetType().GetField("_colors",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (colorsField != null)
        {
            colorsField.SetValue(controller, _candyColors);
        }

        Debug.Log($"[CandySpawner] Конфета спавнена на {randomPosition}");
    }

    // Метод для обновления списка занятых позиций
    private void UpdateOccupiedPositions()
    {
        _occupiedPositions.Clear();

        // Находим все подарки
        PrizeController[] prizes = FindObjectsOfType<PrizeController>();
        foreach (var prize in prizes)
        {
            _occupiedPositions.Add(prize.transform.position);
        }

        // Находим все конфеты
        CandyController[] candies = FindObjectsOfType<CandyController>();
        foreach (var candy in candies)
        {
            _occupiedPositions.Add(candy.transform.position);
        }

        // Находим снеговика (опционально, чтобы не спавнить на нем)
        SnowmanController snowman = FindObjectOfType<SnowmanController>();
        if (snowman != null)
        {
            _occupiedPositions.Add(snowman.transform.position);
        }

        Debug.Log($"[CandySpawner] Найдено занятых позиций: {_occupiedPositions.Count}");
    }

    // Метод для поиска свободной позиции
    private Vector3 GetFreePosition()
    {
        for (int attempt = 0; attempt < _maxAttempts; attempt++)
        {
            Vector3 testPosition = new Vector3(
                Random.Range(-_spawnRange, _spawnRange),
                1f,
                Random.Range(-_spawnRange, _spawnRange)
            );

            bool isFree = true;

            // Проверяем расстояние до всех занятых позиций
            foreach (Vector3 occupiedPos in _occupiedPositions)
            {
                float distance = Vector3.Distance(testPosition, occupiedPos);
                if (distance < _minDistanceFromObjects)
                {
                    isFree = false;
                    break;
                }
            }

            if (isFree)
            {
                Debug.Log($"[CandySpawner] Найдена свободная позиция с {attempt + 1} попытки: {testPosition}");
                return testPosition;
            }
        }

        Debug.LogWarning($"[CandySpawner] Не удалось найти свободную позицию после {_maxAttempts} попыток");
        return Vector3.zero;
    }

    // Опционально: метод для принудительного обновления и спавна
    public void RespawnCandy()
    {
        SpawnCandy();
    }
}