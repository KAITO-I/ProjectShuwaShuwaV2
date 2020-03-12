using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BubbleController : MonoBehaviour {
	Bubble[] bubbles;

	[SerializeField]
	[Range(2.0f, 100.0f)]
	float rateOverTime = 1;

	void Awake()
	{
		var list = new List<Bubble>();
		foreach (Transform child in transform) list.Add(new Bubble(child));
		this.bubbles = list.ToArray();

		foreach (Bubble bubble in this.bubbles) bubble.Enable = true;
	}

	void Update()
	{
		foreach (Bubble bubble in this.bubbles)
		{
			bubble.Update();
			bubble.SetRateOverTime(rateOverTime);
		}
	}
}

class Bubble
{
	const float InstanceTimerMin = 0.2f;
	const float InstanceTimerMax = 1f;

	ParticleSystem particle;

	bool enable = false;
	public bool Enable
	{
		get { return this.enable; }
		set
		{
			this.enable = value;
			if (value)
			{
				this.timer = 0f;
				this.instanceTimer = Random.Range(InstanceTimerMin, InstanceTimerMax);
			}
		}
	}
	
	float timer = 0f;
	float instanceTimer;

	public Bubble(Transform tf)
	{
		this.particle = tf.GetComponent<ParticleSystem>();

		this.instanceTimer = Random.Range(InstanceTimerMin, InstanceTimerMax);
	}

	public void Update()
	{
		if (!this.enable) return;

		this.timer += Time.deltaTime;
		if (this.timer >= this.instanceTimer)
		{
			this.particle.Play();

			this.timer = 0f;
			this.instanceTimer = Random.Range(InstanceTimerMin, InstanceTimerMax);
		}
	}

	public void SetRateOverTime(float rateOverTime)
	{
		var emission = this.particle.emission;
		emission.rateOverTime = rateOverTime;
	}
}