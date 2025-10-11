using System.Collections;
using UnityEngine;

public class GameConsole: MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject defaultSelectedObject;
    private IEnumerator interactionCoroutine;

    public virtual IEnumerator BeginUse()
    {
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
        
        yield return EndUse();
    }
    
    public virtual IEnumerator UseConsole()
    {
        yield return null;
    }

    public virtual IEnumerator EndUse()
    {
        if (canvas != null)
        {
            canvas.enabled = false;
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
        else
        {
            Debug.LogWarning($"No canvas assigned on {gameObject.name}");
        }
        interactionCoroutine = null;
        yield return null;
    }
    

}