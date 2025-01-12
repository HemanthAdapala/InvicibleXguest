using InvicibleXGuest.Scripts.Interfaces;
using UnityEngine;

namespace InvicibleXGuest.Scripts
{
    public class StickyBombAbilityBaseHandler : MonoBehaviour, IAbilityHandler
    {
        private AbilityVisualData _abilityVisualData;
        private GameObject _attachedPowerUpObjectVisual;


        public void Initialize(AbilityItemData abilityItemData)
        {
            _abilityVisualData = abilityItemData.abilityVisualData;
            //var abilityStateMachineManager = PlayerController.LocalInstance.GetComponentInChildren<AbilityStateMachineManager>();
            //abilityStateMachineManager.TransitionToState(abilityStateMachineManager.bombItemAbilityState);
        }

        public void Cleanup()
        {
            Debug.Log("StickyBombAbilityBase Cleanup");
            Destroy(gameObject);
        }

        public void InitializeAbilityHandler()
        {
            // Vector3 playerHeadPosition = PlayerController.LocalInstance.transform.position + Vector3.up * 1.5f;
            // Ray ray = new Ray(playerHeadPosition, PlayerController.LocalInstance.transform.forward);
            // Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
            //
            // if (Physics.Raycast(ray, out RaycastHit hit, 1f, LayerMask.GetMask("SurfaceInteractable")))
            // {
            //     if (CanAttachToSurface(hit.collider.gameObject))
            //     {
            //         ShowVisualPowerUpObject(hit.point, hit.normal);
            //     }
            // }
            // else
            // {
            //     HideVisualClue();
            // }
            //
            // if (Input.GetMouseButtonDown(0) && _attachedPowerUpObjectVisual != null && _attachedPowerUpObjectVisual.activeSelf)
            // {
            //     PlayerManager.LocalInstance.AddNetworkObjectToPlayerServerRpc(AbilityItemType.StickyBomb,_attachedPowerUpObjectVisual.transform.position,_attachedPowerUpObjectVisual.transform.rotation);
            //     Cleanup();
            // }
        }

        private bool CanAttachToSurface(GameObject surface)
        {
            return surface.CompareTag("SurfaceInteractable");
        }

        private void ShowVisualPowerUpObject(Vector3 hitInfoPoint, Vector3 hitInfoNormal)
        {
            if (_attachedPowerUpObjectVisual == null)
            {
                var powerUpObjectVisualInstance = Instantiate(_abilityVisualData.visualEffectPrefab.gameObject);
                _attachedPowerUpObjectVisual = powerUpObjectVisualInstance;
            }

            _attachedPowerUpObjectVisual.gameObject.SetActive(true);
            _attachedPowerUpObjectVisual.gameObject.transform.position = hitInfoPoint;
            _attachedPowerUpObjectVisual.gameObject.transform.rotation = Quaternion.LookRotation(hitInfoNormal);
        }

        private void HideVisualClue()
        {
            if (_attachedPowerUpObjectVisual != null) _attachedPowerUpObjectVisual.gameObject.SetActive(false);
        }
    }
}