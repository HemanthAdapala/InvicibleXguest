using UnityEngine;

public class DistanceVisibilityRule : VisibilityRule
{
    private readonly float _maxDistance;

    public DistanceVisibilityRule(float maxDistance)
    {
        _maxDistance = maxDistance;
    }
    
    public override bool CanSee(GameObject observer, GameObject subject)
    {
        float distance = Vector3.Distance(observer.transform.position, subject.transform.position);
        return distance <= _maxDistance;
    }
}
