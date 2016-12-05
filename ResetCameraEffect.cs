using UnityEngine;
using System.Collections;

public class ResetCameraEffect : MonoBehaviour {
	public float m_ParameterForEffect = 1.0f;
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
		{
			PlayerController.GetInstance().m_ParameterForEfferct = m_ParameterForEffect;
		}
	}
}
