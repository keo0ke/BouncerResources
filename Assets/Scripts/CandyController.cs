using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyController : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Material[] _colors;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 50f;

    private CandySpawner _spawner;

    private void Start()
    {
        ApplyRandomColor();

        _spawner = FindObjectOfType<CandySpawner>();

        if (_spawner == null)
        {
            Debug.LogWarning("[CandyController] Спавнер не найден, создаю новый...");
            GameObject spawnerObject = new GameObject("CandySpawner");
            _spawner = spawnerObject.AddComponent<CandySpawner>();

            var field = _spawner.GetType().GetField("_candyPrefab",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(_spawner, gameObject);
            }
        }
    }

    private void Update()
    {
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
    }

    private void ApplyRandomColor()
    {
        if (_colors == null || _colors.Length == 0)
        {
            Debug.LogError("[CandyController] Нет материалов!");
            return;
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer == null) return;

        Material[] materials = renderer.sharedMaterials; // 🔥 ВАЖНО

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].name.ToLower().Contains("colored"))
            {
                Material randomMat = _colors[Random.Range(0, _colors.Length)];
                materials[i] = randomMat;
                renderer.sharedMaterials = materials; // 🔥

                Debug.Log($"[Candy] Цвет: {randomMat.name}");
                return;
            }
        }

        // fallback
        if (materials.Length > 0)
        {
            Material randomMat = _colors[Random.Range(0, _colors.Length)];
            materials[0] = randomMat;
            renderer.sharedMaterials = materials;
        }
    }

    private Material GetCandyColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer == null) return null;

        Material[] materials = renderer.sharedMaterials; // 🔥

        foreach (var mat in materials)
        {
            if (mat.name.ToLower().Contains("colored"))
            {
                return mat;
            }
        }

        if (materials.Length > 0)
            return materials[0];

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Material candyColor = GetCandyColor();

        SnowmanController snowman = other.GetComponent<SnowmanController>();

        if (snowman != null && candyColor != null)
        {
            snowman.ChangeHatColor(candyColor);
            Debug.Log($"[Candy] Передан цвет: {candyColor.name}");
        }

        Destroy(gameObject);

        if (_spawner != null)
            _spawner.SpawnCandy();
        else
            SpawnCandyManually();
    }

    private void SpawnCandyManually()
    {
        GameObject prefab = Resources.Load<GameObject>("Candy");

        if (prefab == null)
        {
            Debug.LogError("[CandyController] Нет префаба Candy!");
            return;
        }

        Vector3 pos = new Vector3(
            Random.Range(-9f, 9f),
            1f,
            Random.Range(-9f, 9f)
        );

        GameObject newCandy = Instantiate(prefab, pos, Quaternion.identity);

        CandyController controller = newCandy.GetComponent<CandyController>();
        if (controller == null)
            controller = newCandy.AddComponent<CandyController>();

        controller._colors = _colors;
    }

    public void SetColor(int index)
    {
        if (_colors == null || index >= _colors.Length) return;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer == null) return;

        Material[] materials = renderer.sharedMaterials; // 🔥

        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i].name.ToLower().Contains("colored"))
            {
                materials[i] = _colors[index];
                renderer.sharedMaterials = materials; // 🔥
                return;
            }
        }
    }
}