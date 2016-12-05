using UnityEngine;
using System.Collections;

public class CameraCollider : MonoBehaviour
{
	public CameraCtForMoivew m_Camera;
	public CameraType m_Type;
	//public Transform[] m_PosForCameraOne;
	public Transform   m_PosForCameraTwo;
	public Transform   m_PosForCameraThree;
	public float m_SpeedForPath;
	public int m_IndexForCameraOne;
	public Transform m_LookAtObj;
	void Start ()
	{
		
	}
	void Update ()
	{
	
	}
	void OnDrawGizmos()
	{

	}
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "player")
		{
			m_Camera.m_CameraType = m_Type;
			if(m_Type == CameraType.One)
			{
				//m_Camera.m_PosForCameraOne = m_PosForCameraOne;
				m_Camera.m_LookAtObj = m_LookAtObj;
				m_Camera.m_IndexForCameraOne = m_IndexForCameraOne;
			}
			else if(m_Type == CameraType.Two)
			{
				m_Camera.m_PosForCameraTwo = m_PosForCameraTwo;
			}
			else if(m_Type == CameraType.Three)
			{
				m_Camera.m_PosForCameraThree = m_PosForCameraThree;
				m_Camera.m_LookAtObj = m_LookAtObj;
			}
		}
	}
}
