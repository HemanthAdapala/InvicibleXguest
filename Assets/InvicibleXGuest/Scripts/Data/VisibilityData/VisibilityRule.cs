using UnityEngine;

public abstract class VisibilityRule
{
    public abstract bool CanSee(GameObject observer, GameObject subject);
}
