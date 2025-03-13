using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFloor : MonoBehaviour
{
    [SerializeField] private Transform respawn_point;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Object entered trigger: " + other.name);

        if (respawn_point == null)
        {
            Debug.LogError("Respawn point is not assigned in the Inspector!");
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected. Respawning...");

            // Check if the player has a CharacterController
            CharacterController cc = other.GetComponent<CharacterController>();
            if (cc != null)
            {
                cc.enabled = false;
                other.transform.position = respawn_point.position;
                cc.enabled = true;
            }
            else
            {
                // If not using CharacterController, just set position
                other.transform.position = respawn_point.position;
            }

            // Reset velocity if the player has a Rigidbody
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }

            Debug.Log("Player respawned at: " + respawn_point.position);
        }
        else
        {
            Debug.Log("Non-player object detected. No respawn triggered.");
        }
    }
}
