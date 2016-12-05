using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour {

	private Transform tCamera;	//Main Camera transform
	private float fCamShakeImpulse = 0.0f;	//Camera Shake Impulse
	float minShakeVal = 0.05f;

	// Use this for initialization
	void Start () {
		tCamera = camera.transform;
	}

//	int TestVal = 0;
	void Update()
	{

		CameraMain();
	}
	void FixedUpdate()
	{
//		if(Input.GetKeyUp(KeyCode.T))
//		{
//			Debug.Log("*********TestVal " + TestVal);
//			TestVal++;
//			if(TestVal > 5)
//			{
//				TestVal = 1;
//			}
//			setCameraShakeImpulseValue(TestVal);
//		}

		//camera transitions
	}

	/*
	*	FUNCTION: Controls camera movements
	*	CALLED BY: FixedUpdate()
	*/
	private void CameraMain()
	{
		//make the camera shake if the fCamShakeImpulse is not zero
		if(fCamShakeImpulse > 0.0f)
		{
			shakeCamera();
		}
	}
	
	/*
	*	FUNCTION: Make the camera vibrate. Used for visual effects
	*/
	void shakeCamera()
	{
		Vector3 pos = tCamera.position;
		pos.x += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		pos.y += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		pos.z += Random.Range(0, 100) % 2 == 0 ? Random.Range(-fCamShakeImpulse, -minShakeVal) : Random.Range(minShakeVal, fCamShakeImpulse);
		tCamera.position = pos;

		fCamShakeImpulse -= 0.02f * fCamShakeImpulse * 4.0f;
		//Debug.Log("fCamShakeImpulse" + fCamShakeImpulse);
		if(fCamShakeImpulse < minShakeVal)
		{
			fCamShakeImpulse = 0.0f;
		}
	}

	/*
	*	FUNCTION: Set the intensity of camera vibration
	*	PARAMETER 1: Intensity value of the vibration
	*/
	public void setCameraShakeImpulseValue()
	{
		if(fCamShakeImpulse > 0.0f)
		{
			return;
		}
		fCamShakeImpulse = 0.5f;
	}
}
