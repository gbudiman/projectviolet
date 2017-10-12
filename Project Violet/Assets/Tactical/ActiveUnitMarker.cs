using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveUnitMarker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public GameObject spawn(GameObject panel, string name) {
    GameObject prefab_marker = (GameObject)Instantiate(Resources.Load("TimelineMarker"));
    UnityEngine.UI.Text ptext = prefab_marker.GetComponent<UnityEngine.UI.Text>();
    ptext.text = name;
    prefab_marker.transform.SetParent(panel.transform);

    return prefab_marker;
  }
}
