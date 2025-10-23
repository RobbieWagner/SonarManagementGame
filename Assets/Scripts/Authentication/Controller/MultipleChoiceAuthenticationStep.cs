using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.MultiFactorGame
{
	/// <summary>
	/// Authentication step for a multiple-choice view. Validates the integer choice submitted by the view.
	/// </summary>
	public class MultipleChoiceAuthenticationStep : AuthenticationStep
    {
        [SerializeField] private string prompt = "Select the correct option:";
        [SerializeField] private List<string> options;
        [SerializeField] private int correctChoice = 0;
        [SerializeField] private MultipleChoiceView multipleChoiceView;

		public override void StartStep()
		{
			base.StartStep();
            if (authenticationScreen != null)
                authenticationScreen.gameObject.SetActive(true);
            
            multipleChoiceView.StartSelection(options, prompt);
		}

		public override void ContinueStep()
		{
			base.ContinueStep();
            if (authenticationScreen != null)
                authenticationScreen.gameObject.SetActive(true);
                
            multipleChoiceView.StartSelection(options, prompt);
		}

		protected override void ValidateInput(string input)
		{
			// input should be an integer index
			if (string.IsNullOrWhiteSpace(input))
			{
                Debug.Log("Input was empty or whitespace");
			}
			else if (!int.TryParse(input, out int choice))
			{
				Debug.Log("Input was not a valid integer");
			}
			else if (choice == correctChoice)
			{
				EndStep();
			}
			else
			{
				Debug.Log("Input was incorrect");
			}
		}

		private void OnDisable()
		{
			if (authenticationScreen != null)
			{
				authenticationScreen.onInputSubmitted -= ValidateInput;
				authenticationScreen.Clear();
			}
		}
	}
}
