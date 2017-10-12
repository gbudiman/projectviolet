using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalHUD : MonoBehaviour {
  GameObject panel;
  Htecs htecs;
  ActiveUnitMarker active_unit_marker;
  ActiveUnit current_unit;
  ActiveUnit to_back_queue_unit;
  float to_back_time_unit;
  bool action_pending;
	// Use this for initialization
	void Start () {
    panel = GameObject.FindGameObjectWithTag("TagPanel");
    panel.SetActive(false);
    active_unit_marker = GetComponent<ActiveUnitMarker>();
    htecs = new Htecs(active_unit_marker);
    action_pending = false;
	}
	
	// Update is called once per frame
	void Update () {
    query_htecs();
    update_to_back_queue_unit();
	}

  void query_htecs() {
    if (!action_pending) {
      ActiveUnit unit = htecs.get_ready_unit();
      if (unit != null) {
        action_pending = true;
        panel.SetActive(true);
        print("Got " + unit.name);
        current_unit = unit;
      }
    }
  }

  void update_to_back_queue_unit() {
    if (to_back_queue_unit != null) {
      float nextpos = to_back_queue_unit.compute_marker_position(0);
      float xlerp = Mathf.Lerp(300f, nextpos, to_back_time_unit);
      Vector3 current_position = to_back_queue_unit.marker_transform.anchoredPosition3D;
      to_back_queue_unit.marker_transform.anchoredPosition3D = new Vector3(xlerp, current_position.y, current_position.z);

      to_back_time_unit += (Time.deltaTime * 4);

      if (to_back_time_unit > 1f) {
        action_pending = false;
        current_unit.take_action(100f);
        to_back_queue_unit = null;
        htecs.clear_first_ready();
        
        to_back_time_unit = 0f;
      }
    }
  }

  public void take_action(string action) {
    to_back_queue_unit = current_unit;
    to_back_time_unit = 0f;
    panel.SetActive(false);
  }

}
