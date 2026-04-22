using System;
using _Project.Scriptable;
using Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI
{
    [RequireComponent(typeof(Button))]
    public class LevelPrefab : MonoBehaviour
    {
        public static event Action<LevelPrefab> OnClick;
        public LevelData LevelData { get; private set; }
        
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;
        [SerializeField] private Image selection;
        
        private SaveController saveController => SaveController.Instance;
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }

        public void Init(LevelData levelData)
        {
            LevelData = levelData;
            image.sprite = saveController.GetSpriteByScene(levelData.LevelName);
            text.SetText(levelData.LevelName.ToString());
        }

        public void SetSelected(bool isSelected)
        {
            selection.gameObject.SetActive(isSelected);
        }
    }
}