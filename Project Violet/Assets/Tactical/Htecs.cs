using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Htecs {
  bool in_combat;
  protected SortedList<float, ActiveUnit> readies;
  List<ActiveUnit> active_units;
  ActiveUnitMarker active_unit_marker;

  public Htecs(ActiveUnitMarker _active_unit_marker) {
    in_combat = false;
    readies = new SortedList<float, ActiveUnit>();
    active_unit_marker = _active_unit_marker;
    test_harness();

    begin_combat();
	}

  public ActiveUnit get_ready_unit() {
    if (readies.Count == 0) {
      in_combat = true;
      increment_time();
      return null;
    } else {
      float key = readies.Keys[0];
      ActiveUnit au = readies[key];

      //readies.Remove(key);
      return au;
    }
  }

  public void clear_first_ready() {
    readies.RemoveAt(0);

  }

  void begin_combat() {
    in_combat = true;
  }

  void increment_time() {
    //Debug.Log("Incrementing time unit");
    if (in_combat) {
      foreach (ActiveUnit active_unit in active_units) {
        float ap = active_unit.increment_time();
        if (ap > 100f) {
          readies.Add(ap, active_unit);
        }
      }

      if (readies.Count > 0) {
        //Debug.Log("oy ready");
        in_combat = false;
      }
    }
  }

  void test_harness() {
    active_units = new List<ActiveUnit>();
    for (int i = 10; i < 20; i++) {
      active_units.Add(new ActiveUnit(active_unit_marker, "Unit_" + (i).ToString(), i * 2));
    }
  }
}
