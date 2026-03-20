using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowmanController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;

    [Header("Bounce Settings")]
    [SerializeField] private float _bounceForce = 8f;
    [SerializeField] private float _bounceDuration = 0.3f;
    [SerializeField] private float _boundarySize = 9f;

    private Vector3 _targetPosition;
    private bool _isBouncing = false;
    private Vector3 _bounceDirection;
    private float _bounceTimer = 0f;
    private Material _currentHatMaterial;

    private void Start()
    {
        // Устанавливаем начальную позицию
        transform.position = new Vector3(0, 0, 0);
        _targetPosition = transform.position;

        // Добавляем тег для идентификации
        gameObject.tag = "Player";

        // Добавляем Rigidbody для триггеров
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;

        // Настраиваем коллайдер (если его нет)
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider>();
        }
        col.isTrigger = false; // Важно: не триггер!

        Debug.Log($"[SnowmanController] Снеговик инициализирован на позиции {transform.position}");
    }

    private void Update()
    {
        // Режим отталкивания
        if (_isBouncing)
        {
            HandleBounce();
            return;
        }

        // Обычное движение
        HandleMovement();

        ClampPosition();
    }

    private void HandleMovement()
    {
        // Получаем цель по клику мыши
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, transform.position.y);
            float distance;

            if (plane.Raycast(ray, out distance))
            {
                _targetPosition = ray.GetPoint(distance);
                // Ограничиваем цель границами
                _targetPosition.x = Mathf.Clamp(_targetPosition.x, -_boundarySize, _boundarySize);
                _targetPosition.z = Mathf.Clamp(_targetPosition.z, -_boundarySize, _boundarySize);
            }
        }

        // Поворачиваемся к цели
        Vector3 directionToTarget = _targetPosition - transform.position;
        if (directionToTarget.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        // Двигаемся к цели
        transform.position = Vector3.MoveTowards(
            transform.position,
            _targetPosition,
            _speed * Time.deltaTime
        );

        // Визуализация цели в Scene View
        Debug.DrawLine(transform.position, _targetPosition, Color.green);
    }

    private void HandleBounce()
    {
        _bounceTimer -= Time.deltaTime;

        // Двигаемся в направлении отталкивания
        transform.position += _bounceDirection * _bounceForce * Time.deltaTime;

        // Визуализация отталкивания
        Debug.DrawRay(transform.position, _bounceDirection * 2f, Color.red, 0.1f);

        if (_bounceTimer <= 0)
        {
            _isBouncing = false;
            Debug.Log($"[SnowmanController] Отталкивание завершено. Текущая позиция: {transform.position}");

            // После отталкивания обновляем взгляд на цель
            Vector3 directionToTarget = _targetPosition - transform.position;
            if (directionToTarget.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(directionToTarget);
            }
        }
    }

    // Публичный метод для вызова отталкивания из другого скрипта
    public void BounceFromPrize(Vector3 prizePosition, float force)
    {
        if (_isBouncing)
        {
            Debug.Log($"[SnowmanController] Уже отталкиваюсь, пропускаем");
            return;
        }

        // Вычисляем направление от подарка
        _bounceDirection = (transform.position - prizePosition).normalized;
        _bounceDirection.y = 0; // Игнорируем вертикаль

        // Если направление нулевое, используем случайное
        if (_bounceDirection.magnitude < 0.1f)
        {
            _bounceDirection = Random.insideUnitSphere;
            _bounceDirection.y = 0;
            _bounceDirection.Normalize();
            Debug.Log($"[SnowmanController] Направление нулевое, используем случайное: {_bounceDirection}");
        }

        _bounceForce = force;
        _isBouncing = true;
        _bounceTimer = _bounceDuration;

        Debug.Log($"[SnowmanController] ОТТАЛКИВАНИЕ! Направление: {_bounceDirection}, Сила: {force}, Длительность: {_bounceDuration}");
    }

    // В SnowmanController.cs замените метод GetHatColor на этот:

    public Material GetHatColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer == null) return null;

        foreach (var mat in renderer.sharedMaterials) // 🔥
        {
            string name = mat.name.ToLower();

            if (!name.Contains("snow") &&
                !name.Contains("wood") &&
                !name.Contains("carrot") &&
                !name.Contains("ribbon"))
            {
                return mat;
            }
        }

        return null;
    }

    public void ChangeHatColor(Material newColorMaterial)
    {
        if (newColorMaterial == null) return;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
            renderer = GetComponentInChildren<Renderer>();

        if (renderer == null) return;

        Material[] mats = renderer.sharedMaterials; // 🔥

        for (int i = 0; i < mats.Length; i++)
        {
            string name = mats[i].name.ToLower();

            if (!name.Contains("snow") &&
                !name.Contains("wood") &&
                !name.Contains("carrot") &&
                !name.Contains("ribbon"))
            {
                mats[i] = newColorMaterial;
                renderer.sharedMaterials = mats; // 🔥

                Debug.Log($"Шапка → {newColorMaterial.name}");
                return;
            }
        }

        if (mats.Length > 0)
        {
            mats[mats.Length - 1] = newColorMaterial;
            renderer.sharedMaterials = mats;
        }
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