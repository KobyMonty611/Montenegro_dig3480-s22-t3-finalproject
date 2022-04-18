using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedOrb : MonoBehaviour
{
    public AudioClip speedClip;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
                controller.ChangeSpeed();
                Destroy(gameObject);
            
                controller.PlaySound(speedClip);
        }
    }
}
