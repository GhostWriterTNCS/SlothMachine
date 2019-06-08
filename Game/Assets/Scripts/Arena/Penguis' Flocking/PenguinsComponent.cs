using System.Collections;
using UnityEngine;

public abstract class PenguinsComponent : MonoBehaviour
{
    public abstract Vector3 GetDirection(Collider[] neighbors, int size);
}

