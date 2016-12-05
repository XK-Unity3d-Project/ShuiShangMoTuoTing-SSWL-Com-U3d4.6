using UnityEngine;
using System.Collections;

public class AddVelocitySet : MonoBehaviour
{
	public float m_TopSpeedSet = 0.0f;
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
			PlayerController.m_pTopSpeed = m_TopSpeedSet;
		}
	}
}
