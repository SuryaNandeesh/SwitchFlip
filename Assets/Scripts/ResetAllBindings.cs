using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ResetAllBindings : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;

    public void ResetBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }

        // Optional: Remove saved rebinds if they were stored
        PlayerPrefs.DeleteKey("rebinds");
        PlayerPrefs.Save(); // Ensure PlayerPrefs changes are saved
    }
}
