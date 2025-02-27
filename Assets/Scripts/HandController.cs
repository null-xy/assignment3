using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class HandController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Hand hand;
    [Header("Input Actions")]
    public InputActionReference gripAction;
    public InputActionReference triggerAction;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float gripValue = gripAction.action.ReadValue<float>();
        float triggerValue = triggerAction.action.ReadValue<float>();
        hand.SetGrip(gripValue);
        hand.SetTrigger(triggerValue);
    }
}
