using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridMap : MonoBehaviour {
  public Dictionary<int, Dictionary<int, HexadrantTiles>> radius_tiles;
  public Dictionary<int, Dictionary<int, Dictionary<int, GameObject>>> tiles;
  private Camera main_camera;
  private System.Random randomizer;
  const float camera_movement_speed = 0.25f;
  const float zoom_factor = 2f;
  const float mouse_pan_factor = -0.67f;
  private bool debug_all_visible = false;
  int max_radius;
  enum MouseState { LeftPressed, RightPressed, None };
  MouseState mouse_state;

	// Use this for initialization
	void Start () {
    randomizer = new System.Random();
    max_radius = 23;
    make_map(max_radius);
    SkillLoader skl = GetComponent<SkillLoader>();
    skl.Build();
    SkillStruct ss = skl.get_struct();
    main_camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    mouse_state = MouseState.None;

    apply_skills_to_hex_grid_map(ss);
    populate_rest();
    activate_ground_zero();
	}
	
	// Update is called once per frame
	void Update () {
    update_camera();
	}

  private void update_camera() {
    Vector3 current_position = main_camera.transform.position;
    Vector3 target_position = new Vector3(0,0,0);
    if (Input.GetKey(KeyCode.W)) {
      main_camera.transform.Translate(0, camera_movement_speed, 0);
    } else if (Input.GetKey(KeyCode.S)) {
      main_camera.transform.Translate(0, -camera_movement_speed, 0);
    }

    if (Input.GetKey(KeyCode.A)) {
      main_camera.transform.Translate(-camera_movement_speed, 0, 0);
    } else if (Input.GetKey(KeyCode.D)) {
      main_camera.transform.Translate(camera_movement_speed, 0, 0);
    }

    if (Input.GetKeyDown(KeyCode.H)) {
      switch_visibility_state();
    }

    if (Input.GetMouseButtonDown(1)) {
      mouse_state = MouseState.RightPressed;
    }

    if (Input.GetMouseButtonUp(1)) {
      mouse_state = MouseState.None;
    }

    float wheel = Input.GetAxis("Mouse ScrollWheel");
    main_camera.orthographicSize -= (wheel * zoom_factor);

    if (mouse_state == MouseState.RightPressed) {
      main_camera.transform.Translate(Input.GetAxis("Mouse X") * mouse_pan_factor, Input.GetAxis("Mouse Y") * mouse_pan_factor, 0);
    }
  }

  private void switch_visibility_state() {
    foreach (var a in tiles.Keys) {
      foreach (var b in tiles[a].Keys) {
        foreach (var c in tiles[a][b].Keys) {
          tiles[a][b][c].GetComponent<HexGrid>().debug_reveal(!debug_all_visible);
        }
      }
    }

    debug_all_visible = !debug_all_visible;
  }

  private void activate_ground_zero() {
    tiles[0][0][0].GetComponent<HexGrid>().reveal(true);
  }

  public bool has_adjacency_to_revealed_tile(List<List<int>> lints) {
    foreach (List<int> lint in lints) {
      GameObject gt = tiles[lint[0]][lint[1]][lint[2]];
      if (gt.GetComponent<HexGrid>().is_revealed()) return true;
    }

    return false;
  }

  private void make_map(int r) {
    tiles = new Dictionary<int, Dictionary<int, Dictionary<int, GameObject>>>();
    radius_tiles = new Dictionary<int, Dictionary<int, HexadrantTiles>>();

    for (int x = -r; x <= r; x++) {
      for (int y = -r; y <= r; y++) {
        int z = -(x + y);

        if (Mathf.Abs(z) > r) continue;

        if (!tiles.ContainsKey(x)) tiles[x] = new Dictionary<int, Dictionary<int, GameObject>>();
        if (!tiles[x].ContainsKey(y)) tiles[x][y] = new Dictionary<int, GameObject>();
        
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

        tiles[x][y][z] = hex_sprite;

        if (z > r) break;
      }

     
    }
  }

  private void colorize(GameObject hex_sprite, int max_radius) {
    HexGrid hex_grid = hex_sprite.GetComponent<HexGrid>();
    SpriteRenderer sprite = hex_grid.GetComponent<SpriteRenderer>();

    int hexadrant = hex_grid.hexadrant;
    int radius = hex_grid.radius;
    bool is_profession_border = hex_grid.profession_border;

    float hue = 0f;
    float saturation = 0.5f;
    float lightness = (float)radius / (float)max_radius * 0.25f + 0.25f;

    if (hexadrant < 0 || is_profession_border) {
      hue = 0f;
      //lightness = 0.25f;
      saturation = 0f;
      
    } else if (hexadrant < 10) {
      hue = (hexadrant - 1) * 60f / 360f;
    } else {
      int dihex = hexadrant / 10 - 1;
      int mihex = hexadrant % 10 - 1;

      hue = (dihex * 2 + mihex) * 30f / 360f;
      if (mihex == 0) {
        hue += 0.25f * 30f / 360f;
      } else {
        hue -= 0.25f * 30f / 360f;
      }
      
    }

    sprite.color = Color.HSVToRGB(hue, saturation, lightness);
    //float multiplier = ((float) radius / (float) max_radius / 2.0f) + 0.5f;
    //float minorplier = ((float) radius / (float)max_radius) * 3f;

    //switch (hexadrant) {
    //  case 1: sprite.color = new Color(minorplier * 0.25f, multiplier * 0.75f, 0f); break;
    //  case 4: sprite.color = new Color(multiplier * 0.75f, minorplier * 0.25f, 0f); break;
    //  case 2: sprite.color = new Color(0f, minorplier * 0.25f, multiplier * 0.75f); break;
    //  case 5: sprite.color = new Color(0f, multiplier * 0.75f, minorplier * 0.25f); break;
    //  case 3: sprite.color = new Color(multiplier * 0.75f, 0f, minorplier * 0.25f); break;
    //  case 6: sprite.color = new Color(minorplier * 0.25f, 0f, multiplier * 0.75f); break;
    //  default: sprite.color = new Color(minorplier * 0.25f, minorplier * 0.25f, minorplier * 0.25f); break;
    //}
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

  private GameObject get_available_tile(int radius, int hexadrant) {
    HexadrantTiles ht = radius_tiles[radius][hexadrant];
    GameObject gt = ht.select_random(randomizer, true);

    return gt;
  }

  private List<GameObject> get_all_tiles(int radius, int hexadrant, bool mark_used=true) {
    return radius_tiles[radius][hexadrant].get_unused(mark_used);
  }
}

public class HexadrantTiles {
  public List<GameObject> data;
  public List<int> unused_indices;

  public HexadrantTiles() {
    data = new List<GameObject>();
    unused_indices = new List<int>();
  }

  public void Add(GameObject w) {
    data.Add(w);
    unused_indices.Add(unused_indices.Count);
  }

  public GameObject select_random(System.Random randomizer, bool mark_used=false) {
    int l = unused_indices.Count;
    int unusedref = randomizer.Next(0, l);
    int indexref = unused_indices[unusedref];

    if (mark_used) {
      unused_indices.RemoveAt(unusedref);
    }
    
    return data[indexref];
  }

  public List<GameObject> get_unused(bool mark_used=false) {
    List<GameObject> unused = new List<GameObject>();

    foreach (var i in unused_indices) {
      unused.Add(data[i]);
    }

    if (mark_used) {
      unused_indices = new List<int>();
    }

    return unused;
  }
}

public enum TileType { preset_skill, any_stat, fixed_stat, any_skill, profession_border };