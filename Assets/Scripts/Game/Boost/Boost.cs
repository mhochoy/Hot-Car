using UnityEngine;

public class Boost : MonoBehaviour
{
    public float value;
    public float timer;
    bool Activated = false;
    MeshRenderer mesh;
    CapsuleCollider capsuleCollider;

    private void Awake()
    {
        mesh = GetComponentInChildren<MeshRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    protected virtual void Update()
    {
        if (Activated)
        {
            mesh.enabled = false;
            capsuleCollider.enabled = false;
            if (timer > 0.00f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        transform.LookAt(Camera.main.transform);
    }

    public void Activate()
    {
        Activated = true;
    }
}
