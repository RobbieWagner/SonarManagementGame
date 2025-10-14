using System;
using System.Collections;
using RobbieWagnerGames.FirstPerson;
using RobbieWagnerGames.Managers;
using UnityEngine;
using UnityEngine.UI;

public class GameConsole: MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject defaultSelectedObject;
    private IEnumerator interactionCoroutine;

    [SerializeField] private GameObject idleScreen;
    [SerializeField] private GameObject activeScreen;

    [SerializeField] private Transform playerUseTransform;
    [SerializeField] private Transform playerEndUseTransform;

    [SerializeField] private Button closeConsoleButton;

    private void Awake()
    {
        idleScreen.SetActive(true);
        activeScreen.SetActive(false);

        closeConsoleButton.onClick.AddListener(() =>
        {
            if (interactionCoroutine != null)
            {
                StopUsingConsole();
            }
        });

        InputManager.Instance.GetAction(ActionMapName.UI, "Cancel").performed += StopUsingConsole;
    }

    public virtual IEnumerator BeginUse()
    {
        InputManager.Instance.SaveAndDisableCurrentActionMaps();
        InputManager.Instance.EnableActionMap(ActionMapName.UI);
        StartCoroutine(FirstPersonLook.Instance.RotateToWorldOrientationCo(playerUseTransform.rotation, .4f));
        yield return StartCoroutine(FirstPersonMovement.Instance.MoveToWorldPositionCo(playerUseTransform.position, .8f));

        idleScreen.SetActive(false);
        yield return new WaitForSeconds(.25f);
        activeScreen.SetActive(true);

        if (canvas != null)
        {
            canvas.enabled = true;
            if (defaultSelectedObject != null)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(defaultSelectedObject);
            }
            else
            {
                Debug.LogWarning($"No default selected object assigned on {gameObject.name}");
            }
        }
        else
        {
            Debug.LogWarning($"No canvas assigned on {gameObject.name}");
        }
        yield return null;


        interactionCoroutine = UseConsole();
        StartCoroutine(interactionCoroutine);

        while (interactionCoroutine != null)
        {
            yield return null;
        }   
        
        Debug.Log("Ending use of console");
        yield return StartCoroutine(EndUse());
    }
    
    public virtual IEnumerator UseConsole()
    {
        yield return null;
    }

    public virtual IEnumerator EndUse()
    {
        
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        activeScreen.SetActive(false);
        yield return new WaitForSeconds(.25f);
        idleScreen.SetActive(true);
        yield return null;

        StartCoroutine(FirstPersonLook.Instance.RotateToWorldOrientationCo(playerEndUseTransform.rotation, .5f, true));
        yield return StartCoroutine(FirstPersonMovement.Instance.MoveToWorldPositionCo(playerEndUseTransform.position, .5f, true));
        InputManager.Instance.RestoreReservedActionMaps();
    }

    private void StopUsingConsole(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        StopUsingConsole();
    }

    private void StopUsingConsole()
    {
        if (interactionCoroutine != null)
        {
            StopCoroutine(interactionCoroutine);
            interactionCoroutine = null;
        }
    }
}