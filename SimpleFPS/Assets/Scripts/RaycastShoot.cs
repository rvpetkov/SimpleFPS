using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RaycastShoot : MonoBehaviour {

    #region Public members

    public int weaponDamage = 1;
    public float weaponRange = 50f;
    public float fireRate = 0.25f;
    public float hitForce = 100f;
    public Transform weaponEnd;

    [Header("Debugging")]
    public bool debugActive = true;

    #endregion

    #region Private members

    private Camera fpsCamera;
    private AudioSource weaponAudio;
    private LineRenderer laserLine;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private float nextFire;
    RaycastHit hit;
    Vector3 origin;

    #endregion

    // Use this for initialization
    void Start () {
        fpsCamera = GetComponentInParent<Camera>();
        weaponAudio = GetComponent<AudioSource>();
        laserLine = GetComponent<LineRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            StartCoroutine(Shoot());        //Start a coroutine responsible for visualization/audio of the shot
            origin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));   //Origin of the RayCast located in the center of the camera.
            laserLine.SetPosition(0, weaponEnd.position);       //The laserLine will start at the point which represents the end of the weapon.

            if(Physics.Raycast(origin, fpsCamera.transform.forward, out hit, weaponRange))  //if we hit something
            {
                laserLine.SetPosition(1, hit.point);
                ShootableObject target = hit.collider.GetComponentInParent<ShootableObject>();
                if (target != null)
                    target.DealDamage(weaponDamage);

                if(hit.rigidbody != null)
                    hit.rigidbody.AddForce(-hit.normal * hitForce);
            }
            else        //if we don't hit anything
            {
                //we still draw our laser line from the weapon to a point that is directly in front of the camera at a distance of weaponRange.
                laserLine.SetPosition(1, origin + (fpsCamera.transform.forward * weaponRange));
            }

        }
	}

    void FixedUpdate()
    {
        if (debugActive)
        {
            Debug.DrawLine(origin, fpsCamera.transform.forward * weaponRange, Color.green);
        }
    }

    private IEnumerator Shoot()
    {
        //Play the weapon's audio source if any.
        if (weaponAudio != null)
            weaponAudio.Play();

        laserLine.enabled = true;

        yield return shotDuration;      // we wait for some time before the laserLine gets disabled

        laserLine.enabled = false;
    }
}
