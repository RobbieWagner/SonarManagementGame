using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

namespace RobbieWagnerGames.MultiFactorGame
{
    /// <summary>
    /// Simple pin pad component. Exposes 10 number buttons (1..9,0) and a TextMeshProUGUI display.
    /// Implements AuthenticationScreen and will call SubmitInput when the required number of digits are entered.
    /// </summary>
    public class PinPad : AuthenticationScreen
    {
        [Tooltip("Buttons in order 1,2,3,4,5,6,7,8,9,0 (0 should be last)")]
        [SerializeField] private Button[] numberButtons = new Button[10];

        [SerializeField] private TextMeshProUGUI displayText;

        [SerializeField] private int requiredLength = 4;
        [SerializeField] private Button clearButton;

        private UnityAction[] numberActions;

        private void Reset()
        {
            var buttons = GetComponentsInChildren<Button>(true);
            if (buttons != null && buttons.Length >= 10)
            {
                for (int i = 0; i < 10 && i < buttons.Length; i++)
                    numberButtons[i] = buttons[i];
            }
        }

        private void OnEnable()
        {
            WireButtons();
            UpdateDisplay();
        }

        private void OnDisable()
        {
            UnwireButtons();
        }

        public void WireButtons()
        {
            if (numberButtons == null) return;

            numberActions = new UnityAction[numberButtons.Length];
            for (int i = 0; i < numberButtons.Length; i++)
            {
                var btn = numberButtons[i];
                if (btn == null) continue;

                int digit = (i == 9) ? 0 : i + 1; // assuming order 1..9,0
                UnityAction act = () => OnNumberPressed(digit);
                numberActions[i] = act;
                btn.onClick.AddListener(act);
            }

            if (clearButton != null) clearButton.onClick.AddListener(Clear);
        }

        public void UnwireButtons()
        {
            if (numberButtons == null || numberActions == null) return;

            for (int i = 0; i < numberButtons.Length; i++)
            {
                var btn = numberButtons[i];
                var act = i < numberActions.Length ? numberActions[i] : null;
                if (btn == null || act == null) continue;
                btn.onClick.RemoveListener(act);
            }

            numberActions = null;
        }

        private void OnNumberPressed(int digit)
        {
            if (userInput.Length >= requiredLength) return;

            userInput += digit.ToString();
            UpdateDisplay();

            if (userInput.Length >= requiredLength)
            {
                // Submit the input via AuthenticationScreen API so subscribers receive it
                SubmitInput(userInput);
            }
        }

        public override void Clear()
        {
            userInput = string.Empty;
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (displayText == null) return;
            int remaining = Mathf.Max(0, requiredLength - userInput.Length);
            string underscores = new string('_', remaining);
            displayText.text = userInput + underscores;
        }
    }
}
