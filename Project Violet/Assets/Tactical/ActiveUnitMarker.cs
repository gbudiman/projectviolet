using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveUnitMarker : MonoBehaviour {
  HexGridTacticalMap map;
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

  void conditionally_load_map() {
    if (map == null) {
      map = GameObject.FindGameObjectWithTag("htecs").GetComponent<HexGridTacticalMap>();
    }
  }

  public GameObject visualize(HexGridCoord position) {
    conditionally_load_map();

    GameObject parent = map.get_at_abc(position);
    GameObject prefab_unit = (GameObject)Instantiate(Resources.Load("UnitRep"));
    prefab_unit.transform.SetParent(parent.transform);
    prefab_unit.transform.localPosition = new Vector3(0, 0, -0.5f);
    

    return prefab_unit;
  }
}
