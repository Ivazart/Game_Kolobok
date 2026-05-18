using System;
using Global;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Core
{
    public class JumpCountDisplayer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        private JumpsCounterController jumpController => JumpsCounterController.Instance;
        
        private void Awake()
        {
            jumpController.OnJumpsChanged += SaveController_OnJumpCounterChanged;
            DisplayJumps(jumpController.Jumps);
        }

        private void SaveController_OnJumpCounterChanged()
        {
            DisplayJumps(jumpController.Jumps);
        }

        private void DisplayJumps(int count)
        {
            text.text = " " + count;
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