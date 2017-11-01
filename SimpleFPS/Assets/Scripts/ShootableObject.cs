using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableObject : MonoBehaviour {

    public int health = 1;
    [Tooltip("Whis audio will play when the object is destroyed.")]
    public AudioSource audioOnDestroy;

    private WaitForSeconds audioDuration;
    private bool isDead = false;

    void Start()
    {
        if (audioOnDestroy)
            audioDuration = new WaitForSeconds(audioOnDestroy.clip.length);
    }

    public void DealDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);

        if (health <= 0 && !isDead)
        {
            isDead = true;
            StartCoroutine(DieEffect());
        }
    }

    private IEnumerator DieEffect()
    {
        if (audioOnDestroy != null)
            audioOnDestroy.Play();

        yield return audioDuration;

        Destroy(gameObject);
    }
}
