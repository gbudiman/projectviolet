﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridMap : MonoBehaviour {
  public Dictionary<int, Dictionary<int, HexadrantTiles>> radius_tiles;
  

	// Use this for initialization
	void Start () {
    make_map(8);
    SkillLoader skl = GetComponent<SkillLoader>();
    skl.Build();
    SkillStruct ss = skl.get_struct();

    apply_to_hex_grid_map(ss);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  private void make_map(int r) {
    radius_tiles = new Dictionary<int, Dictionary<int, HexadrantTiles>>();

    for (int x = -r; x <= r; x++) {
      for (int y = -r; y <= r; y++) {
        int z = -(x + y);

        if (Mathf.Abs(z) > r) continue;
        GameObject hex_sprite = (GameObject)Instantiate(Resources.Load("HexSprite"));
        HexGrid hex_grid = hex_sprite.GetComponent<HexGrid>();

        Vector3 pl = hex_grid.spawn(x, y, z);
        hex_sprite.transform.position = pl;

        int radius = hex_grid.radius;
        int hexadrant = hex_grid.hexadrant;

        if (!radius_tiles.ContainsKey(radius)) {
          radius_tiles[radius] = new Dictionary<int, HexadrantTiles>();
        }

        if (!radius_tiles[radius].ContainsKey(hexadrant)) {
          radius_tiles[radius][hexadrant] = new HexadrantTiles();
        }

        radius_tiles[radius][hexadrant].Add(hex_sprite);
        colorize(hex_sprite, r);

        if (z > r) break;
      }

     
    }
  }

  private void colorize(GameObject hex_sprite, int max_radius) {
    HexGrid hex_grid = hex_sprite.GetComponent<HexGrid>();
    SpriteRenderer sprite = hex_grid.GetComponent<SpriteRenderer>();

    int hexadrant = hex_grid.hexadrant;
    int radius = hex_grid.radius;

    float multiplier = ((float) radius / (float) max_radius / 2.0f) + 0.5f;
    float minorplier = ((float) radius / (float)max_radius) * 3f;

    switch (hexadrant) {
      case 1: sprite.color = new Color(minorplier * 0.25f, multiplier * 0.75f, 0f); break;
      case 4: sprite.color = new Color(multiplier * 0.75f, minorplier * 0.25f, 0f); break;
      case 2: sprite.color = new Color(0f, minorplier * 0.25f, multiplier * 0.75f); break;
      case 5: sprite.color = new Color(0f, multiplier * 0.75f, minorplier * 0.25f); break;
      case 3: sprite.color = new Color(multiplier * 0.75f, 0f, minorplier * 0.25f); break;
      case 6: sprite.color = new Color(minorplier * 0.25f, 0f, multiplier * 0.75f); break;
      default: sprite.color = new Color(minorplier * 0.25f, minorplier * 0.25f, minorplier * 0.25f); break;
    }
  }

  private void apply_to_hex_grid_map(SkillStruct ss) {
    foreach (KeyValuePair<string, SkillUnit> skill in ss.get_data()) {
      string skill_id = skill.Key;
      SkillUnit skill_data = skill.Value;

      GameObject gt = get_available_tile(skill_data.radius, skill_data.hexadrant);
      TextMesh tm = gt.GetComponentInChildren<TextMesh>();
      tm.text = skill_data.skill_name;
    }
  }

  private GameObject get_available_tile(int radius, int hexadrant) {
    HexadrantTiles ht = radius_tiles[radius][hexadrant];
    GameObject gt = ht.select_random(true);

    return gt;
  }
}

public class HexadrantTiles {
  public List<GameObject> data;
  public List<int> unused_indices;
  private System.Random randomizer;

  public HexadrantTiles() {
    data = new List<GameObject>();
    unused_indices = new List<int>();
    randomizer = new System.Random();
  }

  public void Add(GameObject w) {
    data.Add(w);
    unused_indices.Add(unused_indices.Count);
  }

  public GameObject select_random(bool mark_used=false) {
    int l = unused_indices.Count;
    int unusedref = randomizer.Next(0, l);
    int indexref = unused_indices[unusedref];

    if (mark_used) {
      unused_indices.RemoveAt(unusedref);
    }
    
    return data[indexref];
  }
}
