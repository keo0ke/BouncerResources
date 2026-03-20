using System.Collections;
using UnityEngine;

public class PrizeController : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float _bounceForce = 8f;

    private Material _prizeColor;

    private void Start()
    {
        SetupCollider();
        SetupRigidbody();
        GetPrizeColor();
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

    private void GetPrizeColor()
    {
        Transform lid = transform.Find("lid");

        if (lid == null)
        {
            Debug.LogError("lid не найден!");
            return;
        }

        Renderer renderer = lid.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Renderer не найден!");
            return;
        }

        Material[] mats = renderer.sharedMaterials; // 🔥 ВАЖНО

        foreach (var mat in mats)
        {
            if (mat.name.ToLower().Contains("colored"))
            {
                _prizeColor = mat;
                Debug.Log($"[Prize] Цвет: {mat.name}");
                return;
            }
        }

        if (mats.Length > 0)
            _prizeColor = mats[0];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        SnowmanController snowman = other.GetComponent<SnowmanController>();
        if (snowman == null) return;

        Material hatColor = snowman.GetHatColor();

        Debug.Log($"Hat: {hatColor?.name} | Prize: {_prizeColor?.name}");

        if (hatColor != null && _prizeColor != null && hatColor == _prizeColor)
        {
            Debug.Log("✅ СОВПАЛО → уничтожаем");
            StartCoroutine(DestroyAnimation());
        }
        else
        {
            Debug.Log("❌ НЕ СОВПАЛО → отталкивание");
            snowman.BounceFromPrize(transform.position, _bounceForce);
            StartCoroutine(PrizeHitAnimation());
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

    private IEnumerator PrizeHitAnimation()
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
    }
}