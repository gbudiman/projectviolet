using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticalGrid : HexGrid {
  TacticalHUD tactical_hud;
  GameObject to_move;
  Vector3 old_position, new_position;
  float time_step = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    update_to_move_unit();
	}

  void update_to_move_unit() {
    if (to_move != null) {
      to_move.transform.position = Vector3.Lerp(old_position, new_position, time_step);

      time_step += Time.deltaTime * 2;
      if (time_step > 1f) {
        time_step = 0f;
        unhighlight();
        to_move.transform.SetParent(transform);
        tactical_hud.highlight_active_unit();
        tactical_hud.reevaluate_hovered_tile();
        to_move = null;
      }
    }
  }

  void conditional_load_tactical_hud() {
    if (tactical_hud == null) {
      tactical_hud = GameObject.Find("HUD").GetComponent<TacticalHUD>();
    }
  }

  void OnMouseEnter() {
    on_mouse_enter();
  }

  public void on_mouse_enter() {
    if (has_adjacency_to_active_unit_tile()) {
      highlight(true);
    } else {
      midlight(true);
    }

    tactical_hud.register_hovered_tile(this, true);
  }

  private void OnMouseUp() {
    if (has_adjacency_to_active_unit_tile()) {
      if (is_occupied()) {
        tactical_hud.take_action(100f);
      } else { 
        GameObject active_unit = tactical_hud.get_active_unit();
        if (active_unit != null) {
          active_unit.GetComponentInParent<TacticalGrid>().unhighlight();
          old_position = active_unit.transform.position;
          new_position = transform.position - new Vector3(0, 0, 0.5f);

          active_unit.transform.SetParent(null);
          to_move = active_unit;


        }
      }
    }
  }

  bool is_occupied() {
    return GetComponentInChildren<ActionableEntity>() != null;
  }

  void OnMouseExit() {

    if (!is_currently_active()) {
      //highlight(false);
      unhighlight();
    }
  }

  bool is_currently_active() {
    conditional_load_tactical_hud();
    HexGridCoord active_unit_position = tactical_hud.get_active_unit_position();

    if (active_unit_position != null) {
      return hex_grid_coord.equals(active_unit_position);
    }

    return false;
  }

  bool has_adjacency_to_active_unit_tile() {
    conditional_load_tactical_hud();
    HexGridCoord active_unit_position = tactical_hud.get_active_unit_position();

    if (active_unit_position != null) {
      foreach (HexGridCoord grid in active_unit_position.get_adjacent_grids()) {
        if (grid.equals(hex_grid_coord)) return true;
      }
    }

    return false;
  }
}
