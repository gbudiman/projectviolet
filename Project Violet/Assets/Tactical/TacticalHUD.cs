using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalHUD : MonoBehaviour {
  GameObject panel;
  Htecs htecs;
  ActiveUnitMarker active_unit_marker;
  ActiveUnit current_unit;
  ActiveUnit to_back_queue_unit;
  HexGrid currently_hovered_grid;
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
        current_unit = unit;
        populate_panel();
        highlight_map_tile(current_unit, true);
      }
    }
  }

  public GameObject get_active_unit() {
    if (current_unit != null) {
      return current_unit.map_marker;
    }

    return null;
  }

  public HexGridCoord get_active_unit_position() {
    if (current_unit != null) {
      GameObject map_marker = current_unit.map_marker;
      if (map_marker != null) {
        TacticalGrid tg = map_marker.GetComponentInParent<TacticalGrid>();
        if (tg != null) {
          return tg.hex_grid_coord;
        }
      }

      //return current_unit.map_marker.GetComponent<TacticalGrid>().hex_grid_coord;
    }

    return null;
  }

  void update_to_back_queue_unit() {
    if (to_back_queue_unit != null) {
      float nextpos = to_back_queue_unit.compute_marker_position();
      float xlerp = Mathf.Lerp(300f, nextpos, to_back_time_unit);
      Vector3 current_position = to_back_queue_unit.timeline_marker_transform.anchoredPosition3D;
      to_back_queue_unit.timeline_marker_transform.anchoredPosition3D = new Vector3(xlerp, current_position.y, current_position.z);

      to_back_time_unit += (Time.deltaTime * 4);

      if (to_back_time_unit > 1f) {
        highlight_map_tile(to_back_queue_unit, false);
        action_pending = false;
        to_back_queue_unit = null;
        htecs.clear_first_ready();
        
        to_back_time_unit = 0f;
        
      }
    }
  }

  void highlight_map_tile(ActiveUnit w, bool val) {
    w.map_marker.GetComponentInParent<TacticalGrid>().highlight(val);
  }

  public void register_hovered_tile(HexGrid hx, bool val) {
    if (val) {
      currently_hovered_grid = hx;
    } else {
      currently_hovered_grid = null;
    }
  }

  public void reevaluate_hovered_tile() {
    ((TacticalGrid)currently_hovered_grid).on_mouse_enter();
    currently_hovered_grid = null;
  }

  public void highlight_active_unit() {
    current_unit.map_marker.GetComponentInParent<TacticalGrid>().highlight(true);
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
