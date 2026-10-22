using System;
using UnityEngine;

// Runs an action when its GameObject is destroyed (e.g. a popup closing).
public class OnDestroyCallback : MonoBehaviour
{
    public Action Action;

    void OnDestroy() => Action?.Invoke();
}
