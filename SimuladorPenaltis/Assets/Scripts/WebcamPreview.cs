using UnityEngine;
using UnityEngine.UI;

public class WebcamPreview : MonoBehaviour
{
    [Header("UI")]
    public RawImage preview;
    public AspectRatioFitter aspectRatioFitter;

    [Header("Camera")]
    public string preferredDeviceName = "";
    public int requestedWidth = 1280;
    public int requestedHeight = 720;
    public int requestedFPS = 30;

    private WebCamTexture webcamTexture;

    public WebCamTexture WebcamTexture => webcamTexture;

    private void Start()
    {
        StartCamera();
    }

    private void StartCamera()
    {
        var devices = WebCamTexture.devices;

        if (devices == null || devices.Length == 0)
        {
            Debug.LogError("No se detectó ninguna webcam en Unity.");
            return;
        }

        string selectedDevice = devices[0].name;

        if (!string.IsNullOrWhiteSpace(preferredDeviceName))
        {
            foreach (var device in devices)
            {
                if (device.name.Contains(preferredDeviceName))
                {
                    selectedDevice = device.name;
                    break;
                }
            }
        }

        Debug.Log("Usando webcam: " + selectedDevice);

        webcamTexture = new WebCamTexture(selectedDevice, requestedWidth, requestedHeight, requestedFPS);
        preview.texture = webcamTexture;
        preview.material.mainTexture = webcamTexture;
        webcamTexture.Play();
    }

    private void Update()
    {
        if (webcamTexture == null || !webcamTexture.isPlaying)
            return;

        if (aspectRatioFitter != null && webcamTexture.width > 16)
        {
            float ratio = (float)webcamTexture.width / webcamTexture.height;
            aspectRatioFitter.aspectRatio = ratio;
        }

        // Corrige rotación si la webcam virtual la reporta
        preview.rectTransform.localEulerAngles = new Vector3(0, 0, -webcamTexture.videoRotationAngle);

        // Corrige espejo vertical
        preview.uvRect = webcamTexture.videoVerticallyMirrored
            ? new Rect(0, 1, 1, -1)
            : new Rect(0, 0, 1, 1);
    }

    private void OnDisable()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
