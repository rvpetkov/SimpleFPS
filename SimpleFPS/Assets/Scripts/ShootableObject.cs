using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootableObject : MonoBehaviour {

    public int health = 1;
    [Tooltip("Whis audio will play when the object is destroyed.")]
    public AudioSource audioOnDestroy;

    private WaitForSeconds audioDuration;
    private bool isDead = false;
    private static string SHOT_EFFECTS_PARENT_NAME = "ShotEffects";

    public bool IsDead
    {
        get { return isDead; }
        private set { isDead = value; }
    }

    void Start()
    {
        if (audioOnDestroy)
            audioDuration = new WaitForSeconds(audioOnDestroy.clip.length);
    }

    public void DealDamage(int damage)
    {
        health -= damage;

        if (health <= 0 && !IsDead)
        {
            IsDead = true;
            StartCoroutine(DieEffect());

            Transform shotEffects = transform.Find(SHOT_EFFECTS_PARENT_NAME);
            if (shotEffects != null)
            {
                foreach (Transform item in shotEffects.Cast<Transform>().ToList())
                {
                    item.gameObject.SetActive(false);
                    item.transform.parent = ObjectPool.instance.transform;
                }
            }
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
