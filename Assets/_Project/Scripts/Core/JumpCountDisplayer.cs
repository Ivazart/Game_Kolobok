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
        private JumpsCounterController jumpController => JumpsCounterController.Instance;
        private TextMeshProUGUI tmp;
        private void Awake()
        {
            tmp = GetComponent<TextMeshProUGUI>();
            jumpController.OnJumpsChanged += SaveController_OnJumpCounterChanged;
            DisplayJumps(jumpController.Jumps);
        }

        private void SaveController_OnJumpCounterChanged()
        {
            DisplayJumps(jumpController.Jumps);
        }

        private void DisplayJumps(int count)
        {
            tmp.text = " " + count;
        }
        
        private void OnDestroy()
        {
            try
            {
                jumpController.OnJumpsChanged -= SaveController_OnJumpCounterChanged;
            }
            catch
            {
                // ignored
            }
        }
    }
}