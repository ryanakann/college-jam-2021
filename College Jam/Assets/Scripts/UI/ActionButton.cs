using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour
{
    public Button button;
    public TMP_Text actionName;
    public TMP_Text errorDescription;

    public void SetActionName(string name)
    {
        actionName.SetText(name);
    }

    public void SetErrorDescription(string description)
    {
        errorDescription.SetText(description);
        ActivateDescription(); // sorry
    }

    public void ActivateButton()
    {
        button.interactable = true;
        DeactivateDescription();
    }

    public void DeactivateButton()
    {
        button.interactable = false;
        ActivateDescription();
    }

    private void ActivateDescription()
    {
        errorDescription.gameObject.SetActive(true);
    }

    private void DeactivateDescription()
    {
        errorDescription.gameObject.SetActive(false);
    }
}
