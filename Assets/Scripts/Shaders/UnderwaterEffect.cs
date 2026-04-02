using UnityEngine;
using UnityEngine.Rendering;

public class UnderwaterEffect : MonoBehaviour
{
    public Transform player;
    public float waterLevel = 5f;
    public Volume underwaterVolume; // Drag your Global Volume here
    public float transitionSpeed = 5f;

    void LateUpdate()
    {
        // 1. Camera Follows Player (X and Y)
        if (player != null)
        {
            Vector3 targetPos = new Vector3(player.position.x, player.position.y, -10f);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
        }

        // 2. Check if Camera Lens is underwater
        // If the Camera's Y is below 5, turn on the Volume (weight = 1)
        float targetWeight = (transform.position.y < waterLevel) ? 1f : 0f;
        
        underwaterVolume.weight = Mathf.MoveTowards(underwaterVolume.weight, targetWeight, transitionSpeed * Time.deltaTime);
    }
}