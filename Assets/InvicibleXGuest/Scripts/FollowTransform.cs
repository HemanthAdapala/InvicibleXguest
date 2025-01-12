using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform _targetTransform;

    public void FollowTarget(Transform target)
    {
        _targetTransform = target;

        // Disable collision between KeyObject and Player
        if (target.TryGetComponent<Collider>(out var targetCollider) &&
            TryGetComponent<Collider>(out var keyObjectCollider))
        {
            Physics.IgnoreCollision(keyObjectCollider, targetCollider, true);
        }

        // Optionally disable the collider on the KeyObject
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.enabled = false;
        }
    }

    public void StopFollowing()
    {
        if (_targetTransform != null &&
            _targetTransform.TryGetComponent<Collider>(out var targetCollider) &&
            TryGetComponent<Collider>(out var keyObjectCollider))
        {
            Physics.IgnoreCollision(keyObjectCollider, targetCollider, false);
        }

        // Re-enable the collider on the KeyObject
        if (TryGetComponent<Collider>(out var collider))
        {
            collider.enabled = true;
        }

        _targetTransform = null;
    }

    private void LateUpdate()
    {
        if (_targetTransform != null)
        {
            transform.position = _targetTransform.position + new Vector3(0, 1.5f, 0); // Offset
            transform.rotation = _targetTransform.rotation;
        }
    }
}