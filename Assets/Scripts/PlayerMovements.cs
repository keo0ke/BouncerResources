using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _snowmanPrefab;
    [SerializeField] private GameObject _prizePrefab;
    [SerializeField] private int _prizeCount = 6;
    [SerializeField] private float _spawnRange = 9f;

    private GameObject _spawnedSnowman;

    private void Awake()
    {
        SpawnSnowman();
        SpawnPrizes();
    }

    private void SpawnSnowman()
    {
        if (_snowmanPrefab == null)
        {
            Debug.LogError("[GameBoard] Префаб снеговика не назначен!");
            return;
        }

        _spawnedSnowman = Instantiate(_snowmanPrefab);
        _spawnedSnowman.transform.position = new Vector3(0, 0, 0);

        // Добавляем SnowmanController, если его нет на префабе
        if (_spawnedSnowman.GetComponent<SnowmanController>() == null)
        {
            _spawnedSnowman.AddComponent<SnowmanController>();
            Debug.Log("[GameBoard] SnowmanController добавлен на снеговика");
        }

        Debug.Log($"[GameBoard] Снеговик спавнен на позиции {_spawnedSnowman.transform.position}");
    }

    private void SpawnPrizes()
    {
        if (_prizePrefab == null)
        {
            Debug.LogError("[GameBoard] Префаб подарка не назначен!");
            return;
        }

        for (int i = 0; i < _prizeCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-_spawnRange, _spawnRange),
                0f,
                Random.Range(-_spawnRange, _spawnRange)
            );

            GameObject prize = Instantiate(_prizePrefab);
            prize.transform.position = randomPosition;

            // Добавляем PrizeController, если его нет на префабе
            if (prize.GetComponent<PrizeController>() == null)
            {
                prize.AddComponent<PrizeController>();
            }

            Debug.Log($"[GameBoard] Подарок {i + 1} спавнен на позиции {randomPosition}");
        }

        Debug.Log($"[GameBoard] Всего спавнено {_prizeCount} подарков");
    }

    // Опционально: визуализация зоны спавна
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_spawnRange * 2, 0.1f, _spawnRange * 2));
    }
}