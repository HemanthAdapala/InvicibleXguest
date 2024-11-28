using UnityEngine;

namespace InvicibleXGuest.Scripts
{
    public class RandomPositionGenerator : MonoBehaviour
    {
        public Vector3 GetRandomPositionWithinBounds(Vector3 minBounds, Vector3 maxBounds)
        {
            float randomX = Random.Range(minBounds.x, maxBounds.x);
            float randomY = Random.Range(minBounds.y, maxBounds.y);
            float randomZ = Random.Range(minBounds.z, maxBounds.z);

            return new Vector3(randomX, randomY, randomZ);
        }
    }
}