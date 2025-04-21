using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using TMPro;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera followCamera; // Reference to the follow camera
    public CinemachineCamera mazeViewCamera; // Reference to the maze view camera
    public KeyCode switchKey = KeyCode.Z; // Public field for the key to switch the camera
    public DCController controller; // Reference to the DCController script

    private AudioManager audioManager; // Reference to the AudioManager script

    [SerializeField] private TextMeshProUGUI switchKeyText; // Reference to the UI text element

    private void Start()
    {
        // Get the AudioManager instance
        audioManager = AudioManager.Instance;
    }

    private void Update()
    {
        if (Input.GetKeyUp(switchKey))
        {
            SwitchCamera();
        }
    }

    private void SwitchCamera()
    {
        // Play the sound effect for switching cameras
        audioManager.Play("BigWhoosh");
        // Check which camera currently has higher priority and switch to the other
        if (followCamera.Priority > mazeViewCamera.Priority)
        {
            SwitchToMazeView();
            controller.Freeze(); // Freeze the player when switching to maze view
        }
        else
        {
            SwitchToFollowView();
            controller.Unfreeze(); // Unfreeze the player when switching to follow view
        }
    }

    private void SwitchToMazeView()
    {
        // Set the priority of the maze view camera higher than the follow camera
        followCamera.Priority = 0;
        mazeViewCamera.Priority = 10;
        // Update the UI text to indicate the current camera view
        switchKeyText.text = "לקירוב המצלמה לחץ";
    }

    private void SwitchToFollowView()
    {
        // Set the priority of the follow camera higher than the maze view camera
        followCamera.Priority = 10;
        mazeViewCamera.Priority = 0;
        // Update the UI text to indicate the current camera view
        switchKeyText.text = "להרחקת המצלמה לחץ";
    }
}
