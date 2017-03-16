using UnityEngine;
using System.Collections;

public class XKQiNangTestCtrl : MonoBehaviour
{
	public Texture[] QiNangTexture;
	UITexture QiNangTestTexture;
	// Use this for initialization
	void Start()
	{
		QiNangTestTexture = GetComponent<UITexture>();
	}
	
	// Update is called once per frame
	void Update()
	{
		int indexVal = 0;
		bool isShowTexture = false;
		if (pcvr.m_IsOpneForwardQinang) {
			indexVal = 0;
			isShowTexture = true;
		}

		if (pcvr.m_IsOpneBehindQinang) {
			indexVal = 1;
			isShowTexture = true;
		}

		if (pcvr.m_IsOpneLeftQinang) {
			indexVal = 2;
			isShowTexture = true;
		}

		if (pcvr.m_IsOpneRightQinang) {
			indexVal = 3;
			isShowTexture = true;
		}

		if (isShowTexture) {
			QiNangTestTexture.mainTexture = QiNangTexture[indexVal];
		}
		QiNangTestTexture.enabled = isShowTexture;
	}
}