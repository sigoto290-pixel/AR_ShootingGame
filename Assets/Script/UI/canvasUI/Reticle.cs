using UnityEngine;

public class Reticle : MonoBehaviour
{
    public Camera Camera;
    public Transform GunMuzzleTr;
    public float Distance = 800;

    RectTransform rectTr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTr = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        rectTr.position = Vector3.Scale(Camera.WorldToScreenPoint(GunMuzzleTr.position + GunMuzzleTr.forward * Distance),new Vector3(1, 1, 0));
    }
}
