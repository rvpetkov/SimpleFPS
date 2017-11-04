using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastShoot : MonoBehaviour {

    #region Public members

    public int weaponDamage = 1;
    public float weaponRange = 50f;
    public float fireRate = 0.25f;
    public float hitForce = 100f;
    public LineRenderer laserLine;

    [Tooltip("This is the spot where the shot visual will start from.")]
    public Transform weaponEnd;

    [Header("Debugging")]
    public bool debugActive = true;

    #endregion

    #region Private members

    private Camera fpsCamera;
    private AudioSource weaponAudio;
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private float nextFire;
    RaycastHit hit;
    Vector3 origin;

    private static string SHOTEFFECTS_PARENT_NAME = "ShotEffects";

    #endregion

    // Use this for initialization
    void Start () {
        fpsCamera = GetComponentInParent<Camera>();
        weaponAudio = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;

            laserLine.SetPosition(0, weaponEnd.position);       //The laserLine will start at the point which represents the end of the weapon.

            origin = fpsCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f));   //Origin of the RayCast located in the center of the camera.
            if(Physics.Raycast(origin, fpsCamera.transform.forward, out hit, weaponRange))  //if we hit something
            {
                laserLine.SetPosition(1, hit.point);
            }
            else        //if we don't hit anything
            {
                //we still draw our laser line from the weapon to a point that is directly in front of the camera at a distance of weaponRange.
                laserLine.SetPosition(1, origin + (fpsCamera.transform.forward * weaponRange));
            }
            StartCoroutine(ShootEffect(hit));        //Start a coroutine responsible for visualization/audio of the shot
        }
	}

    void FixedUpdate()
    {
        if (debugActive)
        {
            Debug.DrawLine(origin, fpsCamera.transform.forward * weaponRange, Color.green);
        }
    }

    private IEnumerator ShootEffect(RaycastHit hit)
    {
        if(hit.transform != null)       //if we actually hit something
        {
            bool targetIsAlive = true;

            //Deal damage if the target is ShootableObject
            ShootableObject target = hit.transform.GetComponentInParent<ShootableObject>();
            if (target != null)
            {
                target.DealDamage(weaponDamage);
                if (target.IsDead)
                    targetIsAlive = false;
            }

            //Manage and display the shot's debris if any.
            if (targetIsAlive)
            {
                //Add physics effect if the target has a Rigidbody
                if (hit.rigidbody != null)
                    hit.rigidbody.AddForce(-hit.normal * hitForce);

                GameObject shotDebris = ObjectPool.instance.GetPooledObject();
                if (shotDebris != null)
                {
                    Transform parent = hit.transform.Find(SHOTEFFECTS_PARENT_NAME);     //This is the parent object holding all shot effects.
                    if (parent == null)
                    {
                        parent = new GameObject(SHOTEFFECTS_PARENT_NAME).transform;
                        parent.SetParent(hit.transform);
                    }
                    //           Instantiate(shotDebris, hit.point, Quaternion.LookRotation(hit.normal), parent);
                    shotDebris.transform.position = hit.point;
                    shotDebris.transform.rotation = Quaternion.LookRotation(hit.normal);
                    shotDebris.transform.SetParent(parent);
                    shotDebris.SetActive(true);
                }
            }
        }

        //Play the weapon's audio source if any.
        if (weaponAudio != null)
            weaponAudio.Play();

        //Manage the LineRenderer
        laserLine.enabled = true;

        yield return shotDuration;      // we wait for some time before the laserLine gets disabled

        laserLine.enabled = false;
    }
}
