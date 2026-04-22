using System;
using Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Core
{
    [RequireComponent(typeof(TextMeshProUGUI) )]
    public class JumpCountDisplayer : MonoBehaviour
    {
        private SaveController saveController => SaveController.Instance;
        private TextMeshProUGUI tmp;
        private void Awake()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            saveController.OnJumpCounterChanged += SaveController_OnJumpCounterChanged;
            DisplayJumps(saveController.JumpCounter);
        }

        private void SaveController_OnJumpCounterChanged(int obj)
        {
            DisplayJumps(saveController.JumpCounter);
        }

        private void DisplayJumps(int count)
        {
            tmp.text = "Jumps: " + count;
        }
        
        private void OnDestroy()
        {
            if (saveController)
                saveController.OnJumpCounterChanged -= SaveController_OnJumpCounterChanged;
        }
    }
}