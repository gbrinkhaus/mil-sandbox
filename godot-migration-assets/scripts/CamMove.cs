using UnityEngine;

public class CamMove : MonoBehaviour {

	private float speed = 2.3f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		// irgendwie häßlich, weil die Cam abrupt stoppt
		// if (Input.anyKey) {
			float mHori = Input.GetAxis ("Horizontal") * speed;
			float mVerti = Input.GetAxis ("Vertical") * speed;
			float mZoom = 0.0f;

			if (Input.GetKey (KeyCode.Q)) {
				mZoom += speed;
			}
			if (Input.GetKey (KeyCode.E)) {
				mZoom -= speed;
			}

			MoveCam(mHori, mZoom , mVerti);
		// }

     }

	public void MoveCam(float x, float y, float z)
    {
        /* if (x + y + z == 0) // reduce Debug-Logs
			return;

		Debug.Log("MoveCam x: " + x  + ", y: " + y + ", z: " + z);
		Vector3 pVPort;
		gVars.mc.getViewportCenterPoint(out pVPort);
		Debug.Log("Viewport center point is: " + pVPort);
		gVars.mc.getViewportCenterObject(out pVPort);
		Debug.Log("Viewport center object is: " + pVPort); */

        Vector3 newPos = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z + z);
		transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * speed);
		// }

	}

}
