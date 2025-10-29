using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    private Animator doorAnimator;
    private AudioSource audioSource;
    private bool isOpen = false;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnMouseDown()
    {
        ToggleDoor();
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        if (doorAnimator != null)
            doorAnimator.SetBool("isOpen", isOpen);
        if (audioSource != null && audioSource.clip != null)
            audioSource.Play();
    }
}
