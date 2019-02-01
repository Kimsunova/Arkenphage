using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour {

    public bool isFocus = false;
    Text pickupText;

    private void Awake()
    {
        pickupText = GetComponentInChildren<Text>();
    }

    public virtual void Interact() { }

    public void OnFocused()
    {
        isFocus = true;
        pickupText.enabled = true;
    }

    public void OnDefocused()
    {
        isFocus = false;
        pickupText.enabled = false;
    }
}
