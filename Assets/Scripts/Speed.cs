using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speed : MonoBehaviour
{
    public float boostAmount = 10f;
    public float boostDuration = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                float inputValue = Input.GetAxis("Vertical");
                playerRb.AddForce(playerRb.transform.forward * boostAmount * inputValue, ForceMode.Impulse);
                StartCoroutine(BoostCooldown());
            }
        }
    }

    private IEnumerator BoostCooldown()
    {
        GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(boostDuration);
        GetComponent<Collider>().enabled = true;
    }
}
