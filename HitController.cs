using UnityEngine;
using System.Collections;

public class HitController : MonoBehaviour 
{
	public Rigidbody m_pRigidbody;
	public bool m_IsHit = false;
	private float m_HitTimmer = 0.0f;
	void Update ()
	{
		if(m_IsHit)
		{
			m_HitTimmer+=Time.deltaTime;
		}
		if(m_HitTimmer>0.5f)
		{
			m_IsHit = false;
			m_HitTimmer = 0.0f;
			m_pRigidbody.freezeRotation = true;
		}
	}
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "zhangai")
		{
			m_IsHit = true;
			//Debug.Log("22222222222222222");
			m_pRigidbody.freezeRotation = false;
		}
	}
}
