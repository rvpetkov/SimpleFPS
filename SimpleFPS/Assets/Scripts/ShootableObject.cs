using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootableObject : MonoBehaviour {

    public int health = 1;
    [Tooltip("Whis audio will play when the object is destroyed.")]
    public AudioClip audioOnDestroy;

    [Tooltip("Whis visual effect will be used when the object is destroyed.")]
    public GameObject vfxOnDestroy;

    private WaitForSeconds audioDuration;
    private bool isDead = false;
    private static string SHOT_EFFECTS_PARENT_NAME = "ShotEffects";
    private AudioSource audioSource;

    public bool IsDead
    {
        get { return isDead; }
        private set { isDead = value; }
    }

    void Start()
    {
        if (audioOnDestroy)
        {
            audioSource = GetComponent<AudioSource>();
            audioDuration = new WaitForSeconds(2);
        }
    }

    public void DealDamage(int damage)
    {
        health -= damage;

        if (health <= 0 && !IsDead)
        {
            IsDead = true;

            Transform shotEffects = transform.Find(SHOT_EFFECTS_PARENT_NAME);
            if (shotEffects != null)
            {
                foreach (Transform item in shotEffects.Cast<Transform>().ToList())
                {
                    item.gameObject.SetActive(false);
                    item.transform.parent = ObjectPool.instance.transform;
                }
            }

			StartCoroutine(DieEffect());
        }
    }

    private IEnumerator DieEffect()
    {
        //Use the OnDeath Particle System visual effect if any.
        if (vfxOnDestroy != null)
        {
            var position = transform.GetComponentInChildren<MeshRenderer>().transform.position;
            GameObject vfx = Instantiate(vfxOnDestroy, position, Quaternion.identity);
            Destroy(vfx, 5f);
        }

        //Play the OnDeath audio source if any.
        if (audioOnDestroy != null && audioSource != null)
        {
            audioSource.clip = audioOnDestroy;
			audioSource.loop = false;
            audioSource.Play();
			MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer> ();
			for (int i = 0; i < meshRenderers.Length; i++) {
				meshRenderers [i].enabled = false;
			}
        }

        yield return audioDuration;

        Destroy(gameObject);
    }
}
