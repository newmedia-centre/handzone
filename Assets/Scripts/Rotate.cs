using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

	public float Speed = 100f;
	public Vector3 RotationAxis = new (1, 0, 0);

	void Update () {
		transform.rotation *= Quaternion.Euler(RotationAxis * (Speed * Time.deltaTime));
	}

}
