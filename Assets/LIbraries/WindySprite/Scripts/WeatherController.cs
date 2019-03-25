using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherController : MonoBehaviour
{
	public static WeatherController Instance;
	
	public Vector2 WindForceRange;
	public Vector2 WindChangeRange;
	private float _currentWindChange;

	private float _targetWindForce;
	public float WindForce;

	public void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	
	void Update ()
	{
		WindForce = Mathf.Lerp(WindForce, _targetWindForce, 0.1f);
		_currentWindChange -= Time.deltaTime;
		if (_currentWindChange > 0)
			return;

		_targetWindForce = Random.Range(WindForceRange.x, WindForceRange.y);
		_currentWindChange = Random.Range(WindChangeRange.x, WindChangeRange.y);
	}
}
