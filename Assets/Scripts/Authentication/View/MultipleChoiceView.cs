using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using TMPro;

namespace RobbieWagnerGames.MultiFactorGame
{
	/// <summary>
	/// View for multiple-choice authentication. Wires an array of Buttons where each index represents a choice.
	/// When a button is selected the view will set userInput to the choice index and call SubmitInput.
	/// </summary>
	public class MultipleChoiceView : AuthenticationScreen
    {
        
        [SerializeField] private TextMeshProUGUI promptText;

        [SerializeField] private Button buttonPrefab;
		[SerializeField] private List<Button> choiceButtons = new List<Button>();

		private UnityAction[] choiceActions;

		public override void Clear()
		{
            foreach (Button button in choiceButtons)
            {
                Destroy(button.gameObject);
            }
            choiceButtons.Clear();

            promptText.text = string.Empty;
		}

		public void StartSelection(List<string> options, string prompt)
		{
            Clear();

            promptText.text = prompt;
            for (int i = 0; i < options.Count; i++)
            {
                string optionText = options[i];
                Button button = Instantiate(buttonPrefab, this.transform);
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = optionText;
                }
                int choiceIndex = i; // capture for closure
                UnityAction act = () => OnChoiceSelected(choiceIndex);
                button.onClick.AddListener(act);
                choiceButtons.Add(button);
            }
		}

		private void OnChoiceSelected(int choiceIndex)
		{
			// set userInput to the index string and submit
			userInput = choiceIndex.ToString();
			SubmitInput(userInput);
		}
	}
}