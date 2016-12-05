using UnityEngine;
using System.Collections;

public enum CameraType
{
	One,
	Two,//dingdian bugensui
	Three,//dingdian gensui
	Four
};
public class CameraCtForMoivew : MonoBehaviour 
{
	public CameraType m_CameraType = CameraType.One;
	public Transform[] m_PosForCameraOne;
	public Transform   m_PosForCameraTwo;
	public Transform   m_PosForCameraThree;
	public float m_SpeedForPath = 10.0f;
	public int m_IndexForCameraOne;
	public Transform m_LookAtObj;
	//public GameObject m_GameObject;
	void Start () 
	{
		
	}
//	void Update () 
//	{
//	
//	}
	void FixedUpdate()
	{
		if(m_CameraType == CameraType.One)
		{
			if(m_IndexForCameraOne == 1)
			{
				transform.position = Vector3.Lerp(transform.position,m_PosForCameraOne[m_IndexForCameraOne].position,20.0f*Time.deltaTime*Time.timeScale);

			}
			else if(m_IndexForCameraOne == 7)
			{
				transform.position = Vector3.Lerp(transform.position,m_PosForCameraOne[m_IndexForCameraOne].position,2.0f*Time.deltaTime*Time.timeScale);
			}
			else
			{
				transform.parent = m_PosForCameraOne[m_IndexForCameraOne];
				transform.localPosition = new Vector3(.0f,0.0f,0.0f);
			}
			//transform.position = m_PosForCameraOne[m_IndexForCameraOne].position/*Vector3.Lerp(transform.position,m_PosForCameraOne[m_IndexForCameraOne].position,5.0f* Time.deltaTime)*/;
			//transform.position = m_PosForCameraOne[m_IndexForCameraOne].position;
			transform.LookAt(m_LookAtObj.position);
		}
		else if(m_CameraType == CameraType.Two)
		{
			transform.parent = null;
			transform.position = m_PosForCameraTwo.position;
			transform.localEulerAngles = m_PosForCameraTwo.localEulerAngles;
		}
		else if(m_CameraType == CameraType.Three)
		{
			transform.parent = null;
			transform.position = m_PosForCameraThree.position;
			transform.LookAt(m_LookAtObj);
		}
	}
}
