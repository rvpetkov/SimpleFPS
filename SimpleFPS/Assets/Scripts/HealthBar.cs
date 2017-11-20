using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	public Image healthBar;
	public Text healthBarStatus;

	[SerializeField] private float hitPoints = 100f;
	[SerializeField] private float maxHitPoints = 100f;
	[SerializeField] private Color fullColor = Color.green;
	[SerializeField] private Color lowColor = Color.red;

	void Start () {
		healthBar = GetComponent<Image> ();

		UpdateHealthBar ();
	}

	private void UpdateHealthBar()
	{
		float ratio = hitPoints / maxHitPoints;
		healthBar.transform.localScale = new Vector3 (ratio, 1, 1);
		healthBar.color = Color.Lerp (lowColor, fullColor, ratio);
		if(healthBarStatus != null)
			healthBarStatus.text = (ratio * 100).ToString("0") + '%';
	}

	public void TakeDamage(float damage)
	{
		hitPoints -= damage;
		if (hitPoints < 0)
			hitPoints = 0;

		UpdateHealthBar ();
	}

	public void HealDamage(float healAmount)
	{
		hitPoints += healAmount;
		if (hitPoints > maxHitPoints)
			hitPoints = maxHitPoints;

		UpdateHealthBar ();
	}
}
