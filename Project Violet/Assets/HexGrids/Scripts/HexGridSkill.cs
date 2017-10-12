using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridSkill : HexGridBase {

	// Use this for initialization
	void Start () {
    base.Start();

    SkillLoader skl = GetComponent<SkillLoader>();
    skl.Build();
    SkillStruct ss = skl.get_struct();

    apply_skills_to_hex_grid_map(ss);
    populate_rest();
    activate_ground_zero();
  }
	
	// Update is called once per frame
	void Update () {
    base.Update();
	}

  private void apply_skills_to_hex_grid_map(SkillStruct ss) {
    foreach (KeyValuePair<string, SkillUnit> skill in ss.get_data()) {
      string skill_id = skill.Key;
      SkillUnit skill_data = skill.Value;

      int target_hexadrant = skill_data.hexadrant;
      List<int> hxds = new List<int>();

      if (target_hexadrant == -1) {
        foreach (int k in radius_tiles[skill_data.radius].Keys) {
          hxds.Add(k);
        }
      } else {
        hxds.Add(target_hexadrant);
      }

      foreach (int hxd in hxds) {
        if (skill_data.bias_method == "border") {
          foreach (GameObject gt in get_all_tiles(skill_data.radius, hxd)) {
            HexGrid hg = gt.GetComponent<HexGrid>();
            TextMesh tm = gt.GetComponentInChildren<TextMesh>();
            hg.profession_border = true;
            colorize(gt, max_radius);

            tm.text = skill_data.skill_name;
            //hg.set_color(TileType.profession_border, max_radius);
          }
        } else {
          GameObject gt = get_available_tile(skill_data.radius, hxd);
          HexGrid hg = gt.GetComponent<HexGrid>();
          TextMesh tm = gt.GetComponentInChildren<TextMesh>();
          tm.text = skill_data.skill_name;
          //hg.set_color(TileType.preset_skill, max_radius);
        }

      }

    }
  }

  private void populate_rest() {
    foreach (KeyValuePair<int, Dictionary<int, HexadrantTiles>> radius_tile in radius_tiles) {
      foreach (KeyValuePair<int, HexadrantTiles> hexadrant_tile in radius_tile.Value) {
        List<GameObject> remaining_tiles = hexadrant_tile.Value.get_unused();

        foreach (var tile in remaining_tiles) {
          int randval = randomizer.Next(0, 100);
          TextMesh tm = tile.GetComponentInChildren<TextMesh>();
          HexGrid hg = tile.GetComponent<HexGrid>();

          if (radius_tile.Key < 9) {
            tm.text = "Any Stat";
          } else {
            if (randval < 70) {
              tm.text = "Maj Stat";
              //hg.set_color(TileType.fixed_stat, max_radius);
            } else {
              tm.text = "Any Stat";
              //hg.set_color(TileType.any_stat, max_radius);
            }
          }
        }

      }
    }
  }

  private void activate_ground_zero() {
    tiles[0][0][0].GetComponent<HexGrid>().reveal(true);
  }
}
