using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger2 : MonoBehaviour
{
	public Color redcolor;
	public Color bluecolor;
	void Start()
	{
		transform.GetComponent<Renderer>().material.color = bluecolor;
	}

	void OnTriggerEnter(Collider collision)
	{
		transform.GetComponent<Renderer>().material.color = redcolor;
		Counter.pegcounter += 1;
		Counter.recordData = true;
		Counter.isNewTry = true;
		// Counter.currentWires.Add(gameObject);
	}

	void OnTriggerExit(Collider collision)
	{
		Counter.recordData = false;
		Counter.currentWires.Remove(gameObject);
		transform.GetComponent<Renderer>().material.color = bluecolor;

	}


}
