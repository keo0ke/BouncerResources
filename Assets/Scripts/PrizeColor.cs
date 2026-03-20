using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeColor : MonoBehaviour
{
    [SerializeField] private Material[] _colors; // Красный, синий, зеленый материалы
    [SerializeField] private string _coloredMaterialName = "colored"; // Имя материала для замены

    private void Start()
    {
        if (_colors == null || _colors.Length == 0)
        {
            Debug.LogError("[PrizeColorSimple] Материалы не назначены!");
            return;
        }

        // Находим крышку
        Transform lid = transform.Find("lid");

        if (lid != null)
        {
            Renderer renderer = lid.GetComponent<Renderer>();

            if (renderer != null)
            {
                // Получаем все материалы
                Material[] materials = renderer.materials;
                bool materialChanged = false;

                // Ищем материал с нужным именем
                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i].name.Contains(_coloredMaterialName))
                    {
                        // Выбираем случайный цвет
                        Material randomMaterial = _colors[Random.Range(0, _colors.Length)];
                        materials[i] = randomMaterial;
                        materialChanged = true;
                        Debug.Log($"[PrizeColorSimple] Материал '{_coloredMaterialName}' заменен на {randomMaterial.name}");
                        break;
                    }
                }

                // Применяем измененные материалы
                if (materialChanged)
                {
                    renderer.materials = materials;
                }
                else
                {
                    Debug.LogWarning($"[PrizeColorSimple] Материал с именем '{_coloredMaterialName}' не найден!");
                }
            }
        }
    }
}