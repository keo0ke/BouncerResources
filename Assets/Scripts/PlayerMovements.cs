using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _prizePrefab;
    [SerializeField] private float _speed = 5f;

    private Vector3 _targetPosition;

    private void Awake()
    {
        _player = Instantiate(_player);
        _player.transform.position = new Vector3(0, 0, 0);
        _targetPosition = _player.transform.position;

        for (int i = 0; i < 6; i++)
        {
            var prize = Instantiate(_prizePrefab);
            prize.transform.position = new Vector3(Random.Range(-9f, 9f), 0f, Random.Range(-9f, 9f));
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Создаем плоскость на уровне игрока
            var plane = new Plane(Vector3.up, _player.transform.position.y);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;

            if (plane.Raycast(ray, out distance))
            {
                _targetPosition = ray.GetPoint(distance);
            }
        }

        _player.transform.LookAt(_targetPosition);

        // Двигаемся к цели
        _player.transform.position = Vector3.MoveTowards(
            _player.transform.position,
            _targetPosition,
            _speed * Time.deltaTime
        );
    }
}