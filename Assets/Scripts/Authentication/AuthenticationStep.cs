using System.Collections;
using UnityEngine;

namespace RobbieWagnerGames.MultiFactorGame
{
    public class AuthenticationStep : MonoBehaviour
    {
        [SerializeField] private GameObject stepUI;
        [SerializeField] private GameObject defaultSelectedObject;

        private bool _isStepComplete;
        public bool isStepComplete
        {
            get
            {
                return _isStepComplete;
            }
            protected set
            {
                if (_isStepComplete == value) return;
                _isStepComplete = value;
                if (_isStepComplete)
                {
                    onStepCompleted?.Invoke();
                }
            }
        }

        public delegate void StepCompleted();
        public event StepCompleted onStepCompleted;

        public virtual void StartStep()
        {
            Debug.Log("Starting authentication step: " + gameObject.name);
        }

        public virtual void EndStep()
        {
            Debug.Log("Ending authentication step: " + gameObject.name);
            isStepComplete = true;
        }
    }
}