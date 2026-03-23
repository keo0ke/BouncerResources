using System.Collections;
using UnityEngine;

public class SnowmanController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;

    [Header("Bounce Settings")]
    [SerializeField] private float _bounceDuration = 0.3f;
    [SerializeField] private float _boundarySize = 9f;

    private Vector3 _targetPosition;
    private bool _isBouncing = false;
    private Vector3 _bounceDirection;
    private float _bounceTimer = 0f;
    private float _currentBounceForce; // Сила текущего отталкивания
    private Renderer _snowmanRenderer;
    private int _hatMaterialIndex = -1;

    private void Start()
    {
        transform.position = Vector3.zero;
        _targetPosition = transform.position;
        gameObject.tag = "Player";

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if (col == null) col = gameObject.AddComponent<BoxCollider>();
        col.isTrigger = false;

        _snowmanRenderer = GetComponentInChildren<Renderer>();
        _hatMaterialIndex = ColorMaterialUtils.FindMutableMaterialIndex(_snowmanRenderer, "hat", "ribbon", "colored");

        Debug.Log($"[SnowmanController] Снеговик инициализирован на позиции {transform.position}");
    }

    private void Update()
    {
        if (_isBouncing)
        {
            HandleBounce();
            return;
        }

        HandleMovement();
        ClampPosition();
    }

    private void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, transform.position.y);

            if (plane.Raycast(ray, out float distance))
            {
                _targetPosition = ray.GetPoint(distance);
                _targetPosition.x = Mathf.Clamp(_targetPosition.x, -_boundarySize, _boundarySize);
                _targetPosition.z = Mathf.Clamp(_targetPosition.z, -_boundarySize, _boundarySize);
            }
        }

        Vector3 directionToTarget = _targetPosition - transform.position;
        if (directionToTarget.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        transform.position = Vector3.MoveTowards(transform.position, _targetPosition, _speed * Time.deltaTime);
        Debug.DrawLine(transform.position, _targetPosition, Color.green);
    }

    private void HandleBounce()
    {
        _bounceTimer -= Time.deltaTime;
        transform.position += _bounceDirection * _currentBounceForce * Time.deltaTime;
        Debug.DrawRay(transform.position, _bounceDirection * 2f, Color.red, 0.1f);

        if (_bounceTimer <= 0)
        {
            _isBouncing = false;
            Debug.Log($"[SnowmanController] Отталкивание завершено. Позиция: {transform.position}");

            Vector3 directionToTarget = _targetPosition - transform.position;
            if (directionToTarget.magnitude > 0.1f)
                transform.rotation = Quaternion.LookRotation(directionToTarget);
        }
    }

    public void BounceFromPrize(Vector3 prizePosition, float force)
    {
        if (_isBouncing) return;

        _bounceDirection = (transform.position - prizePosition).normalized;
        _bounceDirection.y = 0;
        if (_bounceDirection.magnitude < 0.1f)
        {
            _bounceDirection = Random.insideUnitSphere;
            _bounceDirection.y = 0;
            _bounceDirection.Normalize();
        }

        _currentBounceForce = force;
        _isBouncing = true;
        _bounceTimer = _bounceDuration;

        Debug.Log($"[SnowmanController] Отталкивание! Направление: {_bounceDirection}, Сила: {force}, Длительность: {_bounceDuration}");
    }

    public Material GetHatColor()
    {
        if (_snowmanRenderer == null) return null;
        Material[] mats = _snowmanRenderer.materials;
        if (_hatMaterialIndex < 0 || _hatMaterialIndex >= mats.Length) return null;
        return mats[_hatMaterialIndex];
    }

    public ColorId GetHatColorId()
    {
        return ColorMaterialUtils.GetColorId(GetHatColor());
    }

    public void ChangeHatColor(Material newColorMaterial)
    {
        if (newColorMaterial == null) return;

        if (_snowmanRenderer == null) return;

        Material[] mats = _snowmanRenderer.materials;
        if (_hatMaterialIndex < 0 || _hatMaterialIndex >= mats.Length) return;

        mats[_hatMaterialIndex] = newColorMaterial;
        _snowmanRenderer.materials = mats;
        Debug.Log($"[SnowmanController] Шапка изменена на {newColorMaterial.name}");
    }

    private void ClampPosition()
    {
        float x = Mathf.Clamp(transform.position.x, -_boundarySize, _boundarySize);
        float z = Mathf.Clamp(transform.position.z, -_boundarySize, _boundarySize);
        transform.position = new Vector3(x, transform.position.y, z);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_targetPosition, 0.3f);

            if (_isBouncing)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position, _bounceDirection * 2f);
                Gizmos.DrawWireSphere(transform.position + _bounceDirection * 2f, 0.2f);
            }
        }
    }
}