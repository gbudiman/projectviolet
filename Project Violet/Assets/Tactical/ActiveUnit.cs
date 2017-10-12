using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActiveUnit{
  public float action_point;
  public float ap_increment;
  public string name;
  GameObject panel;
  protected GameObject marker;
  public RectTransform marker_transform;
  ActiveUnitMarker active_unit_marker;
  protected float timeline_length = 670 + 300;
  protected float segment = 5f;
	// Use this for initialization

  public ActiveUnit(ActiveUnitMarker _active_unit_marker, string _name, float _ap_increment = 20f) {
    panel = GameObject.FindGameObjectWithTag("TagTimeline");
    name = _name;
    ap_increment = _ap_increment;
    active_unit_marker = _active_unit_marker;

    add_timeline_marker();
  }

  void add_timeline_marker() {
    marker = active_unit_marker.spawn(panel, name);
    marker_transform = marker.GetComponent<RectTransform>();
    marker_transform.anchoredPosition = new Vector3(0, -330, 0);
    //marker = new Vector3(0f, -330f, 0f);
  }

  void update_position_on_timeline() {
    set_marker_position(compute_marker_position());
  }

  void set_marker_position(float x) {
    marker_transform.anchoredPosition = new Vector3(x, -330, 0);
  }

  public Dictionary<string, KeyValuePair<string, float>> generate_actions() {
    Dictionary<string, KeyValuePair<string, float>> actions = new Dictionary<string, KeyValuePair<string, float>>();

    actions.Add("Skip Turn", new KeyValuePair<string, float>("skip", 100f));
    actions.Add("Defend", new KeyValuePair<string, float>("defend", 50f));
    actions.Add("Attack", new KeyValuePair<string, float>("attack", 100f));

    return actions;
  }

  public float compute_marker_position() {
    float estimation = (100f - action_point) / ap_increment;
    if (estimation > segment) {
      return -670f;
    } else {
      return timeline_length - ((estimation / segment) * timeline_length) - 670;
    }
  }
  
  public float increment_time() {
    action_point += ap_increment * Time.deltaTime * 2;
    update_position_on_timeline();
    if (action_point >= 100f) {
      return action_point;
    }

    return -1;
  }

  public float take_action(float ap_cost) {
    action_point -= ap_cost;
    return action_point;
  }
}
