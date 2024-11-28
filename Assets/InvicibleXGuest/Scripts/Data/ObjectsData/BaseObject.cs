using Unity.Netcode;
using UnityEngine;

public class BaseObject : NetworkBehaviour
{
    public ObjectType objectType;
    public int id;

    public void Initialize(int id, ObjectType objectType)
    {
        this.id = id;
        this.objectType = objectType;
    }
}

public enum ObjectType
{
    Key,
    Door
}