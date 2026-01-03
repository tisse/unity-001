using System.Collections.Generic;
using System.Linq;
using Level;
using Hero;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Movee : MonoBehaviour
{
    private Camera[] _cameras;

    private List<Hero.Hero> _heroes;

    [SerializeField] private float moveSpeed = 5f;

    private InputAction _movementAction;

    private InputAction _cycleAction;

    private InputAction _cycleCameraAction;

    private int _currentSelectionIndex;

    private int _currentCameraIndex;

    private TMP_Text _selectedObjectNameText;
    private TMP_Text _selectedObjectHealthText;

    private Dictionary<GameObject, HeroData> _heroData;
    private Dictionary<string, HeroData> _heroStatData;

    [SerializeField] private TMP_FontAsset font;

    private readonly HudTextParameter _hudTextParameter = new HudTextParameter()
    {
        RowHeight = 48,
        RowPadding = 16
    };

    private void Start()
    {
        LevelJsonLoader.LoadJsonLevel(1);
    }


    private void Awake()
    {
        _cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

        foreach (var cam in _cameras)
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(false);
            }
        }

        _currentCameraIndex = 0;

        _cameras[_currentCameraIndex].gameObject.SetActive(true);

        CreateUI();

        var jsonFile = Resources.Load<TextAsset>("data/hero_data");
        Debug.Log($"hero_data: jsonFile: {jsonFile.text}");
        if (jsonFile != null)
        {
            var data = JsonConvert.DeserializeObject<List<BaseHeroData>>(jsonFile.text);
            data.ForEach(item => Debug.Log($"HeroClass: {item.HeroClass}, HeroClassData: {item.Data}"));
            _heroStatData = data.ToDictionary(hero => hero.HeroClass, hero => hero.Data);
        }
        else
        {
            _heroStatData = new Dictionary<string, HeroData>();
        }

        jsonFile = Resources.Load<TextAsset>("data/spells");
        Debug.Log($"jsonFile: {jsonFile.text}");
        if (jsonFile != null)
        {
            var data = JsonConvert.DeserializeObject<List<SpellDictionary>>(jsonFile.text);
            Debug.Log($"spells: {data.Count}");
            data.ForEach(item => Debug.Log($"spell: {item}"));
        }

        _heroes = new List<Hero.Hero>();
        jsonFile = Resources.Load<TextAsset>("data/hero_classes");
        Debug.Log($"hero_classes: jsonFile: {jsonFile.text}");
        if (jsonFile != null)
        {
            var data = JsonConvert.DeserializeObject<List<ClassData>>(jsonFile.text);
            Debug.Log($"spells: {data.Count}");
            for (var i = 0; i < data.Count; i++)
            {
                _heroes.Add(CreateHero(data[i], i));
            }
        }

        _heroData = new Dictionary<GameObject, HeroData>();

        SelectObject(0);

        _movementAction = new InputAction("Movement");

        _movementAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
        _cycleAction = new InputAction(
            name: "Cycle",
            type: InputActionType.Button,
            binding: "<Keyboard>/tab"
        );
        _cycleAction.performed += OnCyclePerformed;

        _cycleAction.Enable();

        _cycleCameraAction = new InputAction(
            name: "CycleCamera",
            type: InputActionType.Button,
            binding: "<Keyboard>/1"
        );

        _cycleCameraAction.performed += OnCycleCameraPerformed;
        _cycleCameraAction.Enable();
    }

    private void OnCycleCameraPerformed(InputAction.CallbackContext context)
    {
        // Выключаем текущую камеру
        if (_currentCameraIndex >= 0
            && _currentCameraIndex < _cameras.Length
            && _cameras[_currentCameraIndex] != null
            && _cameras[_currentCameraIndex].enabled)
        {
            _cameras[_currentCameraIndex].gameObject.SetActive(false);
        }

        _currentCameraIndex = (_currentCameraIndex + 1) % _cameras.Length;
        _cameras[_currentCameraIndex].gameObject.SetActive(true);
    }

    private void SwitchToCamera(int index)
    {
        if (index < 0 || index >= _cameras.Length)
        {
            Debug.LogWarning($"Неверный индекс камеры: {index}");
            return;
        }

        if (_cameras[index] == null)
        {
            Debug.LogWarning($"Камера с индексом {index} равна null!");
            return;
        }


        // Включаем новую камеру
        _cameras[index].gameObject.SetActive(true);
        _currentCameraIndex = index;

        // Обновляем AudioListener
        // UpdateAudioListener();

        Debug.Log($"Переключено на камеру {index + 1}: {_cameras[index].gameObject.name}");
    }

    private void OnCyclePerformed(InputAction.CallbackContext context)
    {
        if (_heroes.Count == 0) return;

        // Снимаем выделение с текущего
        if (_currentSelectionIndex >= 0)
        {
            DeselectObject(_currentSelectionIndex);
        }

        // Переходим к следующему
        _currentSelectionIndex = (_currentSelectionIndex + 1) % _heroes.Count;

        // Выделяем новый
        SelectObject(_currentSelectionIndex);
    }

    private TMP_Text CreateHUDText(GameObject hudPanel, int row)
    {
        var textObj = new GameObject("TMP_Text");
        textObj.transform.SetParent(hudPanel.transform);
        // Добавляем компонент TMP_Text
        var hudText = textObj.AddComponent<TextMeshProUGUI>();

        // Настраиваем текст
        hudText.text = "Hello!";
        hudText.fontSize = 48;
        hudText.color = Color.white;
        hudText.alignment = TextAlignmentOptions.Center;

        // Настраиваем RectTransform
        var rect = textObj.GetComponent<RectTransform>();
        // 5. ПРИВЯЗКА К ЛЕВОМУ ВЕРХНЕМУ УГЛУ:
        rect.anchorMin = new Vector2(0, 1); // Левый верхний угол
        rect.anchorMax = new Vector2(0, 1); // Левый верхний угол
        rect.pivot = new Vector2(0, 1); // Точка привязки - левый верх

        // 6. Отступы от края (в пикселях)
        rect.anchoredPosition = new Vector2(20, -_hudTextParameter.RowPadding - _hudTextParameter.RowHeight * row);

        // 7. Размер
        rect.sizeDelta = new Vector2(300, 50);
        return hudText;
    }

    private void CreateUI()
    {
        var canvas = FindFirstObjectByType<Canvas>();

        var hudPanel = CreateTopLeftPanel(canvas.transform, new Vector2(250, 150));

        _selectedObjectNameText = CreateHUDText(hudPanel, 0);
        _selectedObjectHealthText = CreateHUDText(hudPanel, 1);
    }

    private static GameObject CreateTopLeftPanel(Transform parent, Vector2 size)
    {
        var panel = new GameObject("TopLeftPanel");
        panel.transform.SetParent(parent, false);

        var bg = panel.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.7f);

        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(20, -20);
        rect.sizeDelta = size;

        return panel;
    }

    private void CycleSelection()
    {
        if (_heroes.Count == 0) return;

        // Снимаем выделение с текущего
        if (_currentSelectionIndex >= 0)
        {
            DeselectObject(_currentSelectionIndex);
        }

        // Переходим к следующему
        _currentSelectionIndex = (_currentSelectionIndex + 1) % _heroes.Count;

        // Выделяем новый
        SelectObject(_currentSelectionIndex);
    }

    private void SelectObject(int index)
    {
        if (index < 0 || index >= _heroes.Count) return;
        ChangeObjectColor(index, Color.green);
        var hero = _heroes[index];
        _selectedObjectNameText.text = hero.ClassName;
        _selectedObjectNameText.color = hero.Color;
        _selectedObjectHealthText.text = hero.HeroData.Health.ToString();
    }

    private void ChangeObjectColor(int index, Color color)
    {
        var obj = _heroes[index].AssignedGameObject;
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    private void DeselectObject(int index)
    {
        if (index < 0 || index >= _heroes.Count) return;
        ChangeObjectColor(index, _heroes[index].Color);
        _selectedObjectNameText.text = "";
    }

    private void OnEnable()
    {
        _movementAction.Enable();
    }

    private void OnDisable()
    {
        _movementAction.Disable();
    }

    private void Update()
    {
        var input = _movementAction.ReadValue<Vector2>();
        var movement = new Vector3(input.x, 0, input.y);

        if (movement == Vector3.zero) return;
        var obj = _heroes[_currentSelectionIndex].AssignedGameObject;
        obj.transform.Translate(movement * moveSpeed * Time.deltaTime);
    }

    private Hero.Hero CreateHero(ClassData classData, int counter)
    {
        var data = _heroStatData[classData.Name];

        return new Hero.Hero
        {
            ClassName = classData.Name,
            AssignedGameObject = GameObjectUtils.CreateGameObject(classData, counter),
            HeroData = new HeroData()
            {
                Agility = data.Agility,
                Health = data.Health,
                Intelligence = data.Intelligence,
                Stamina = data.Stamina,
                ActionPoints = data.ActionPoints,
                Armor = data.Armor,
                Strength = data.Strength,
            },
            Color = classData.GetColor()
        };
    }

    private class HudTextParameter
    {
        public int RowHeight;
        public int RowPadding;
    }
}