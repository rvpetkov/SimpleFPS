﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableObject : MonoBehaviour {

    public int health = 1;

	public void DealDamage(int damage)
    {
        health -= damage;
        Debug.Log(health);

        if (health <= 0)
            Destroy(gameObject);
    }
}
