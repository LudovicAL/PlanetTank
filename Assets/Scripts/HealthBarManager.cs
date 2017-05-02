using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour {

	[Tooltip("Icons displayed as the tank gets destroyed.")] public Sprite[] tankIcons;
	[Tooltip("Color of the healthbar when the tank is in mint condition.")] public Color intact;
	[Tooltip("Color of the healthbar when the tank is most damaged.")] public Color damaged;
	private Image tankIcon;
	private Image healthBar;

	void Start () {
		tankIcon = this.transform.FindChild ("TankIcon").gameObject.GetComponent<Image>();
		healthBar = this.transform.FindChild ("HealthBar").gameObject.GetComponent<Image>();
	}

	/// <summary>
	/// Updates the healthbar fill ratio, icon and color according to the tank current hit points.
	/// </summary>
	public void UpdateHealthBar(int currentHealth, int maxHealth) {
		float ratio = (float)currentHealth / (float)maxHealth;
		healthBar.fillAmount = Mathf.Clamp(ratio, 0.01f, 1.0f);
		tankIcon.sprite = tankIcons[Mathf.Clamp (currentHealth, 0, tankIcons.Length)];
		Color color = Color.Lerp (damaged, intact, ratio);
		tankIcon.color = color;
		healthBar.color = color;
	}
}
