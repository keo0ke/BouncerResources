using System.Collections;
using UnityEngine;

public class PrizeController : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField] private Material[] _colors; // Цвета лент

    [Header("Bounce Settings")]
    [SerializeField] private float _bounceForce = 8f;

    private Material _prizeColor;
    private Renderer _lidRenderer;
    private int _ribbonMaterialIndex = -1;
    private bool _isProcessingHit;

    private void Start()
    {
        SetupCollider();
        SetupRigidbody();
        CacheLidRenderer();
        ApplyRandomColor();
    }

    public void Initialize(Material[] colors)
    {
        if (colors != null && colors.Length > 0)
            _colors = colors;
    }

    private void SetupCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
            col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void SetupRigidbody()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    private void ApplyRandomColor()
    {
        if (_colors == null || _colors.Length == 0)
        {
            Debug.LogError("[PrizeController] Массива цветов нет или он пуст!");
            return;
        }

        if (_lidRenderer == null || _ribbonMaterialIndex < 0)
        {
            Debug.LogError("[PrizeController] Не удалось подготовить изменяемый материал ленты!");
            return;
        }
        Material[] materials = _lidRenderer.materials;
        Material randomMat = _colors[Random.Range(0, _colors.Length)];
        materials[_ribbonMaterialIndex] = randomMat;
        _lidRenderer.materials = materials;
        _prizeColor = randomMat;
        Debug.Log($"[PrizeController] Цвет ленты установлен: {randomMat.name}");

        if (_prizeColor == null)
        {
            Debug.LogError("[PrizeController] Не удалось установить цвет подарка!");
        }
    }

    public Material GetPrizeColor() => _prizeColor;

    private void CacheLidRenderer()
    {
        Transform lid = transform.Find("lid");
        if (lid == null)
        {
            Debug.LogError("[PrizeController] Объект 'lid' не найден!");
            return;
        }

        _lidRenderer = lid.GetComponent<Renderer>();
        if (_lidRenderer == null)
        {
            Debug.LogError("[PrizeController] Рендерер на 'lid' отсутствует!");
            return;
        }

        _ribbonMaterialIndex = ColorMaterialUtils.FindMutableMaterialIndex(_lidRenderer, "ribbon", "colored");
    }

    private Material GetCurrentPrizeColorMaterial()
    {
        if (_lidRenderer == null) return _prizeColor;
        Material[] materials = _lidRenderer.materials;
        if (_ribbonMaterialIndex < 0 || _ribbonMaterialIndex >= materials.Length) return _prizeColor;
        return materials[_ribbonMaterialIndex];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isProcessingHit) return;
        if (!other.CompareTag("Player")) return;

        SnowmanController snowman = other.GetComponent<SnowmanController>();
        if (snowman == null) return;

        Material prizeColor = GetCurrentPrizeColorMaterial();
        _prizeColor = prizeColor;
        Material hatColor = snowman.GetHatColor();

        ColorId prizeColorId = ColorMaterialUtils.GetColorId(prizeColor);
        ColorId hatColorId = snowman.GetHatColorId();

        if (prizeColorId != ColorId.Unknown &&
            hatColorId != ColorId.Unknown &&
            prizeColorId == hatColorId)
        {
            _isProcessingHit = true;
            Debug.Log($"✅ Цвета совпали! Шапка: {hatColorId}, Подарок: {prizeColorId}. Уничтожаем.");
            StartCoroutine(DestroyAnimation());
        }
        else
        {
            _isProcessingHit = true;
            Debug.Log($"❌ Цвета не совпали. Шапка: {hatColorId}, Подарок: {prizeColorId}. Отталкивание.");
            snowman.BounceFromPrize(transform.position, _bounceForce);
            StartCoroutine(PrizeHitAnimationAndUnlock());
        }
    }

    private IEnumerator DestroyAnimation()
    {
        float t = 0;
        Vector3 start = transform.localScale;

        while (t < 0.2f)
        {
            transform.localScale = Vector3.Lerp(start, Vector3.zero, t / 0.2f);
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator PrizeHitAnimationAndUnlock()
    {
        Vector3 start = transform.localScale;
        float t = 0;

        while (t < 0.2f)
        {
            float scale = 1 + Mathf.Sin(t * Mathf.PI / 0.2f) * 0.3f;
            transform.localScale = start * scale;
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = start;
        _isProcessingHit = false;
    }
}