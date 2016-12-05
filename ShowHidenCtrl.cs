using UnityEngine;
using System.Collections;

public class ShowHidenCtrl : MonoBehaviour
{
	public static bool m_IsOpen = false;
	private float timmer = 0.0f;
	public UITexture m_Texture;
	private float m_Apla = 0.0f;
	void Start () 
	{
		m_Texture.alpha = m_Apla;
	}
	void Update () 
	{

		if(m_IsOpen)
		{
			timmer+=Time.deltaTime/Time.timeScale;
			if(timmer<=1.0f)
			{
				m_Texture.alpha += Time.deltaTime/Time.timeScale;
			}
			else if(timmer>1.5f && timmer<2.5f)
			{
				m_Texture.alpha -= Time.deltaTime/Time.timeScale;
			}
			else if(timmer>=2.5f)	
			{
				m_Texture.alpha = 0.0f;
				m_IsOpen = false;
				timmer = 0.0f;
			}
		}
	}
//	public void OnFinishedHiden()
//	{
//
//		//m_Hiden.enabled = false;
//	}
//	public void OnFinishedShow()
//	{
//		m_Texture.alpha = 0.0f;
//		m_IsOpen = false;
//		timmer = 0.0f;
//		m_Show.enabled = false;
//		m_Hiden.enabled = false;
//		m_Hiden.ResetToBeginning();
//		m_Show.ResetToBeginning();
//		gameObject.SetActive(false);
//	}
}
