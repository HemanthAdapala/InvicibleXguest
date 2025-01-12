using UnityEngine;

public class DoorHandler : MonoBehaviour, IDoor
{
    [SerializeField] private Color[] colors;
    private Collider _collider;
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        InitializeComponents();
    }


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player")) Debug.Log("Player entered");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("MeshRenderer initialized");

        // if(other.gameObject.CompareTag("Player"))
        // {
        //     Debug.Log("MeshRenderer initialized");
        //
        //     var player = other.gameObject.GetComponent<PlayerController>();
        //     if(player.HasKey())
        //     {
        //         UpdateDoorState();
        //     }
        // }
    }

    private void InitializeComponents()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();

        InitializeCollider(_collider);
        InitializeMeshRenderer(_meshRenderer);
    }

    private void InitializeCollider(Collider collider)
    {
        Debug.Log("Collider initialized");
    }

    private void InitializeMeshRenderer(MeshRenderer meshRenderer)
    {
        Debug.Log("MeshRenderer initialized");
        meshRenderer.material.color = colors[0];
    }

    private void UpdateDoorState()
    {
        _collider.isTrigger = true;
        _meshRenderer.material.color = colors[1];
    }
}