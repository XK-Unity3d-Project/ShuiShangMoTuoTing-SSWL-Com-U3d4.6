using UnityEngine;
using System.Collections;

public class CutCamera : MonoBehaviour {

	public string m_OffPathName = "";
	public string m_StarPathName = "";
	public GameObject m_Event;
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
			iTweenEvent.GetEvent(m_Event,m_OffPathName).Stop();
			iTweenEvent.GetEvent(m_Event,m_StarPathName).Play();
		}
	}
}
