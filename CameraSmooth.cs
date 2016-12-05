using UnityEngine;
using System.Collections;

public class CameraSmooth : MonoBehaviour 
{
	public Transform target;
//	public float m_LookIndexZ = 1.0f;
//	public float m_LookIndexY = 1.0f;
//	public float m_DistanceMin = 12.5f;
	public float m_DistanceMax = 17.5f;
	public float LookAtIndex = 6.0f;
	public float PositionForward = 6.0f;
	public float PositionUp = 5.0f;
	public float speed = 3.0f;
	void Start () 
	{

        PositionForward = 5.0f; //gzknu
        PositionUp = 2.0f;
        speed = 25.0f;
    }

	void Update () 
	{

	}
	void FixedUpdate()
	{
		if (!target)
			return;
		transform.position = Vector3.Lerp(transform.position,target.position - target.forward*PositionForward+target.up*PositionUp,speed*Time.deltaTime);
		transform.LookAt (target.position + target.forward*LookAtIndex);
	}
	void LateUpdate()
	{
//		if (!target)
//			return;
//		transform.position = Vector3.Lerp(transform.position,target.position - target.forward*PositionForward+target.up*PositionUp,speed*Time.deltaTime);
//		transform.LookAt (target.position + target.forward*LookAtIndex);
	}
	//public Transform target = null;  




//	public float height = 1f;  
//	public float positionDamping = 3f;  
//	public float velocityDamping = 3f;  
//	public float distance = 4f;  
//	public LayerMask ignoreLayers = -1;  
//	
//	private RaycastHit hit = new RaycastHit();  
//	
//	private Vector3 prevVelocity = Vector3.zero;  
//	private LayerMask raycastLayers = -1;  
//	
//	private Vector3 currentVelocity = Vector3.zero; 
//	private float m_speedFactorRecord = 0.0f;
//	private bool m_IsLimit = false;
//	private float m_TimmerLimit = 0.0f;
//	
//	void Start()  
//	{  
//		raycastLayers = ~ignoreLayers;  
//		m_speedFactorRecord = Mathf.Clamp01(target.root.rigidbody.velocity.magnitude*3.6f / 70.0f);
//	}  
//	
//	void FixedUpdate()  
//	{  
//		currentVelocity = Vector3.Lerp(prevVelocity, target.root.rigidbody.velocity, velocityDamping * Time.deltaTime);  
//		currentVelocity.y = 0;  
//		prevVelocity = currentVelocity;  
//	}  
//	
//	void LateUpdate()  
//	{  
//		float speedFactor = Mathf.Clamp01(target.root.rigidbody.velocity.magnitude*3.6f / 70.0f); 
//		if(m_speedFactorRecord - speedFactor > 0.4f && !m_IsLimit)
//		{
//			m_IsLimit = true;
//		}
//		if(m_IsLimit)
//		{
//			m_TimmerLimit+=Time.deltaTime;
//			speedFactor = Mathf.Lerp(m_speedFactorRecord,0.0f,Time.deltaTime*5.0f);
//			if(m_TimmerLimit>1.0f)
//			{
//				m_IsLimit = false;
//				m_TimmerLimit = 0.0f;
//			}
//		}
//		m_speedFactorRecord = speedFactor;
////		if(speedFactor<0.3f)
////		{
////			speedFactor = 0.3f;
////		}
////		camera.fieldOfView = Mathf.Lerp(35, 45, speedFactor);  
////		float currentDistance = Mathf.Lerp(7.5f, 12.5f, speedFactor); 
//		//camera.fieldOfView = Mathf.Lerp(35, 45, speedFactor);  
//		float currentDistance = Mathf.Lerp(m_DistanceMin, m_DistanceMax, speedFactor);  
////		Debug.Log ("currentDistance" + currentDistance);
//		currentVelocity = currentVelocity.normalized;
//
//		Debug.Log("currentVelocity" +currentVelocity);
//		Vector3 newTargetPosition = target.position + Vector3.up * height; 
//		Vector3 newPosition = newTargetPosition - (currentVelocity * currentDistance);  
//
//
//		newPosition.y = newTargetPosition.y;  
//		
//		Vector3 targetDirection = newPosition - newTargetPosition;  
//		if(Physics.Raycast(newTargetPosition, targetDirection, out hit, currentDistance, raycastLayers))  
//			newPosition = hit.point;  
//		if(target.root.rigidbody.velocity.magnitude*3.6f >12.0f)
//		{
//			transform.position = newPosition;  
//			Vector3 lookatPos = target.position + target.forward * m_LookIndexZ - Vector3.up*m_LookIndexY;
//			transform.LookAt(lookatPos); 
//		}
//
//	} 

}
