using UnityEngine;
using System.Collections;

public class ResetWaterSpeed : MonoBehaviour
{
	void Start () 
	{
	
	}
	void Update ()
	{
	
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
		{
			PlayerController.m_LimitSpeed = 0.0f;
		}
	}
}
