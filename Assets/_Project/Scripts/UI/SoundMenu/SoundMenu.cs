using System;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.UI
{
    public class SoundMenu : MonoBehaviour
    {
        [SerializeField] private Button menuButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private GameObject soundMenu;

        private void Awake()
        {
            menuButton.onClick.AddListener( ()=>soundMenu.SetActive(true));
            closeButton.onClick.AddListener(()=> soundMenu.SetActive(false));
        }
    }
}