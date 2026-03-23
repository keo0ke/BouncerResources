using System.Collections;
using UnityEngine;

public class CandyController : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Material[] _colors;

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSpeed = 50f;

    [Header("Spawn Area")]
    [SerializeField] private float _spawnRange = 9f;

    private Collider _candyCollider;
    private Renderer _candyRenderer;
    private int _colorMaterialIndex = -1;

    private void Start()
    {
        _candyCollider = GetComponent<Collider>();
        _candyRenderer = GetComponentInChildren<Renderer>();
        _colorMaterialIndex = ColorMaterialUtils.FindMutableMaterialIndex(_candyRenderer, "colored", "candy", "ribbon");
        ApplyRandomColor();
        RandomizePosition();
    }

    public void Initialize(Material[] colors)
    {
        if (colors != null && colors.Length > 0)
            _colors = colors;
    }

    private void Update()
    {
        transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
    }

    private void ApplyRandomColor()
    {
        if (_colors == null || _colors.Length == 0)
        {
            Debug.LogError("[CandyController] Нет материалов для конфеты!");
            return;
        }

        if (_candyRenderer == null || _colorMaterialIndex < 0) return;

        Material[] materials = _candyRenderer.materials;
        if (_colorMaterialIndex >= materials.Length) return;

        Material randomMat = _colors[Random.Range(0, _colors.Length)];
        materials[_colorMaterialIndex] = randomMat;
        _candyRenderer.materials = materials;
        Debug.Log($"[Candy] Конфета получила цвет: {randomMat.name}");
    }

    private Material GetCandyColor()
    {
        if (_candyRenderer == null) return null;
        Material[] mats = _candyRenderer.materials;
        if (_colorMaterialIndex < 0 || _colorMaterialIndex >= mats.Length) return null;
        return mats[_colorMaterialIndex];
    }

    private void RandomizePosition()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(-_spawnRange, _spawnRange),
            1f,
            Random.Range(-_spawnRange, _spawnRange)
        );
        transform.position = randomPos;
        Debug.Log($"[Candy] Конфета перемещена в {randomPos}");
    }

    public void RelocateAndRecolor()
    {
        RandomizePosition();
        ApplyRandomColor();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Material candyColor = GetCandyColor();
        SnowmanController snowman = other.GetComponent<SnowmanController>();

        if (snowman != null && candyColor != null)
        {
            snowman.ChangeHatColor(candyColor);
            Debug.Log($"[Candy] Цвет шапки изменён на {candyColor.name}");
        }

        // Временно отключаем коллайдер, чтобы избежать повторного срабатывания при перемещении
        if (_candyCollider != null)
            _candyCollider.enabled = false;

        RelocateAndRecolor();

        // Включаем коллайдер в следующем кадре
        StartCoroutine(EnableColliderNextFrame());
    }

    private IEnumerator EnableColliderNextFrame()
    {
        yield return null; // ждём один кадр
        if (_candyCollider != null)
            _candyCollider.enabled = true;
    }
}