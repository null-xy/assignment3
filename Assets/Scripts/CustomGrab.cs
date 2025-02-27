using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomGrab : MonoBehaviour
{

    CustomGrab otherHand = null;
    public List<Transform> nearObjects = new List<Transform>();
    public Transform grabbedObject = null;
    public InputActionReference action;
    public InputActionReference secondAction;
    bool grabbing = false;
    public bool otherHandHolding;

    private Vector3 previousPosition;
    private Quaternion previousRotation;
    public bool doubleRotationEnabled = false;

    private List<(Collider, Collider)> ignoredCollisions = new List<(Collider, Collider)>();
    private Rigidbody grabbedRigidbody = null;

    [Header("Throwing Settings")]
    public float throwForceMultiplier = 1f;
    private Vector3 _currentHandVelocity;
    private Vector3 _currentHandAngularVelocity;
    private void Start()
    {
        action.action.Enable();
        if (secondAction != null)
            secondAction.action.Enable();
        previousPosition = transform.position;
        previousRotation = transform.rotation;

        foreach (CustomGrab c in transform.parent.GetComponentsInChildren<CustomGrab>())
        {
            if (c != this)
                otherHand = c;
        }
    }

    void Update()
    {
        _currentHandVelocity = (transform.position - previousPosition) / Time.deltaTime;

        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        _currentHandAngularVelocity = (axis * angle * Mathf.Deg2Rad) / Time.deltaTime;
        //otherHandHasSameObject = (otherHand.grabbedObject != null && grabbedObject != null
        //                  && otherHand.grabbedObject == grabbedObject);
        if (secondAction != null && secondAction.action.WasPressedThisFrame())
        {
            //doubleRotationEnabled = !doubleRotationEnabled;
            //Debug.Log("Double Rotation: " + doubleRotationEnabled);
        }
        grabbing = action.action.IsPressed();

        if (grabbing)
        {
            if (!grabbedObject)
            {
                grabbedObject = nearObjects.Count > 0 ? nearObjects[0] : otherHand.grabbedObject;
                //otherHandHasSameObject = (otherHand.grabbedObject != null && otherHand.grabbedObject.position == grabbedObject.position);
                if (grabbedObject != null)
                {

                    SetIgnoreCollisions(grabbedObject.gameObject, true);
                    grabbedRigidbody = grabbedObject.GetComponent<Rigidbody>();
                    if (grabbedRigidbody != null)
                    {
                        grabbedRigidbody.useGravity = false;
                        grabbedRigidbody.isKinematic = true;
                    }
                }
            }

            if (grabbedObject)
            {
                //Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);

                //if (doubleRotationEnabled)
                //{
                //    float angle;
                //    Vector3 axis;
                //    deltaRotation.ToAngleAxis(out angle, out axis);
                //    angle *= 2;
                //    deltaRotation = Quaternion.AngleAxis(angle, axis);
                //}
                if (doubleRotationEnabled)
                {
                    deltaRotation *= deltaRotation;
                }

                Vector3 previousOffset = grabbedObject.position - previousPosition;
                Vector3 rotatedOffset = deltaRotation * previousOffset;
                grabbedObject.position = transform.position + rotatedOffset;

                grabbedObject.rotation = deltaRotation * grabbedObject.rotation;
            }
        }
        else if (grabbedObject)
        {
            SetIgnoreCollisions(grabbedObject.gameObject, false);
            bool otherHandHolding = otherHand.grabbedObject == grabbedObject;

            if (grabbedRigidbody != null && !otherHandHolding)
            {
                grabbedRigidbody.useGravity = true;
                grabbedRigidbody.isKinematic = false;
                grabbedRigidbody.linearVelocity = _currentHandVelocity * throwForceMultiplier;
                grabbedRigidbody.angularVelocity = _currentHandAngularVelocity * throwForceMultiplier;
                //grabbedRigidbody = null;
            }
            grabbedRigidbody = null;
            grabbedObject = null;
        }

        previousPosition = transform.position;
        previousRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform t = other.transform;
        if (t && t.tag.ToLower() == "grabbable")
            nearObjects.Add(t);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform t = other.transform;
        if (t && t.tag.ToLower() == "grabbable")
            nearObjects.Remove(t);
    }
    ////////////////////////////////////////////////
    private void SetIgnoreCollisions(GameObject obj, bool ignore)
    {
        // Get all colliders in the hand
        Collider[] handColliders = GetComponentsInChildren<Collider>();
        // Get all colliders on the grabbed object
        Collider[] objectColliders = obj.GetComponentsInChildren<Collider>();

        // If we're turning ignoring ON, we store pairs so we can revert them easily later.
        // If turning ignoring OFF, we remove from the list as we revert them.
        if (ignore)
        {
            ignoredCollisions.Clear();
        }

        foreach (Collider handCol in handColliders)
        {
            // Typically ignore the "trigger" collider used for detection
            // (since it won't push anyway), but that's optional
            if (handCol.isTrigger)
                continue;

            foreach (Collider objCol in objectColliders)
            {
                // If the object collider is also a trigger, it might not matter, but let's skip it
                if (objCol.isTrigger)
                    continue;

                Physics.IgnoreCollision(handCol, objCol, ignore);

                // Keep track if we're ignoring, so we can revert later
                if (ignore)
                {
                    ignoredCollisions.Add((handCol, objCol));
                }
            }
        }

        // If we are re-enabling collisions
        if (!ignore)
        {
            // Revert collisions for all previously ignored pairs
            foreach (var pair in ignoredCollisions)
            {
                if (pair.Item1 != null && pair.Item2 != null)
                {
                    Physics.IgnoreCollision(pair.Item1, pair.Item2, false);
                }
            }

            // Clear the list
            ignoredCollisions.Clear();
        }
    }
}