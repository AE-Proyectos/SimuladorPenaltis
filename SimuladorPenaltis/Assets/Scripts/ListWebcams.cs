using UnityEngine;

public class ListWebcams : MonoBehaviour
{
    private void Start()
    {
        var devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            Debug.Log("No hay webcams detectadas.");
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log($"Cam {i}: {devices[i].name}");
        }
    }
}
