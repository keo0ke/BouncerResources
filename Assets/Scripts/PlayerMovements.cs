using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _snowmanPrefab;
    [SerializeField] private GameObject _prizePrefab;
    [SerializeField] private Material[] _prizeColors;
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

        if (_spawnedSnowman.GetComponent<SnowmanController>() == null)
            _spawnedSnowman.AddComponent<SnowmanController>();

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

            // Убедимся, что есть PrizeController
            PrizeController controller = prize.GetComponent<PrizeController>();
            if (controller == null)
                controller = prize.AddComponent<PrizeController>();
            controller.Initialize(_prizeColors);

            // Цвет будет установлен в Start контроллера, но если нужно сразу,
            // можно вызвать публичный метод инициализации, но в Start уже всё сделано.

            Debug.Log($"[GameBoard] Подарок {i + 1} спавнен на позиции {randomPosition}");
        }

        Debug.Log($"[GameBoard] Всего спавнено {_prizeCount} подарков");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(_spawnRange * 2, 0.1f, _spawnRange * 2));
    }
}