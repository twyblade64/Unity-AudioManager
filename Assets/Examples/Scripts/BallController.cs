using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {
    public AudioClip bounceClip;

    // Load the clip before using it
    public void Start() {
        AudioManager.instance.LoadFX(bounceClip);
    }

    // Warp the ball within a 12x20 origin-aligned square
    void FixedUpdate () {
        Vector3 position = transform.position;
        if (position.y < -6)
            position.y += 12;
        if (position.x > 10)
            position.x -= 20;
        if (position.x < -10)
            position.x += 20;
        transform.position = position;
	}

    // Play the clip on collision
    void OnCollisionEnter2D(Collision2D collision) {
        AudioManager.instance.PlayFX(bounceClip);
    }
}
