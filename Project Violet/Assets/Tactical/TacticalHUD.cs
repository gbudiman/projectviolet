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
        populate_panel();
      }
    }
  }

  void update_to_back_queue_unit() {
    if (to_back_queue_unit != null) {
      float nextpos = to_back_queue_unit.compute_marker_position();
      float xlerp = Mathf.Lerp(300f, nextpos, to_back_time_unit);
      print(xlerp + " => " + nextpos);
      Vector3 current_position = to_back_queue_unit.marker_transform.anchoredPosition3D;
      to_back_queue_unit.marker_transform.anchoredPosition3D = new Vector3(xlerp, current_position.y, current_position.z);

      to_back_time_unit += (Time.deltaTime * 4);

      if (to_back_time_unit > 1f) {
        action_pending = false;
        to_back_queue_unit = null;
        htecs.clear_first_ready();
        
        to_back_time_unit = 0f;
      }
    }
  }

  public void take_action(float action_cost) {
    current_unit.take_action(action_cost);
    to_back_queue_unit = current_unit;
    to_back_time_unit = 0f;

    panel.SetActive(false);
  }

  void populate_panel() {
    Dictionary<string, KeyValuePair<string, float>> actions = current_unit.generate_actions();

    foreach (Transform child in panel.transform) {
      GameObject.Destroy(child.gameObject);
    }

    int button_count = 0;
    foreach (KeyValuePair<string, KeyValuePair<string, float>> action in actions) {
      GameObject button_group = (GameObject)Instantiate(Resources.Load("TacticalButton"));
      UnityEngine.UI.Button button = button_group.GetComponent<UnityEngine.UI.Button>();
      UnityEngine.UI.Text text = button_group.GetComponentInChildren<UnityEngine.UI.Text>();
      RectTransform rect = button_group.GetComponent<RectTransform>();
      Vector3 original_position = rect.anchoredPosition3D;

      text.text = action.Key;
      button.onClick.AddListener(() => take_action(action.Value.Value));
      button_group.transform.SetParent(panel.transform);
      rect.anchoredPosition3D = new Vector3(original_position.x, original_position.y + 40f * button_count, 0);

      button_count++;
    }
  }

}
