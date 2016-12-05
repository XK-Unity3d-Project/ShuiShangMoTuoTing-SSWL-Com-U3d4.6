using UnityEngine;
using System.Collections;

public class FeiyuePubuSet : MonoBehaviour
{
	public float m_PubuPower = 0.0f;
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
			PlayerController.m_IsPubu = true;
			PlayerController.m_PubuPower = m_PubuPower;
		}
	}
}
