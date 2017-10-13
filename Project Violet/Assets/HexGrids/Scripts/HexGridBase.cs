using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridBase : MonoBehaviour {
  public Dictionary<int, Dictionary<int, HexadrantTiles>> radius_tiles;
  public Dictionary<int, Dictionary<int, Dictionary<int, GameObject>>> tiles;
  private Camera main_camera;
  protected System.Random randomizer;
  const float camera_movement_speed = 0.25f;
  protected float zoom_factor = 2f;
  protected float mouse_pan_factor = -0.67f;
  private bool debug_all_visible = false;
  protected int max_radius;
  enum MouseState { LeftPressed, RightPressed, None };
  MouseState mouse_state;

  protected delegate void ColorizeMethod(GameObject g, int r);

	// Use this for initialization
	protected void Start () {
    randomizer = new System.Random();
    max_radius = 23;
    //make_map(max_radius);
    
    main_camera = GameObject.Find("Main Camera").GetComponent<Camera>();
    mouse_state = MouseState.None;

    
	}
	
	// Update is called once per frame
	protected void Update () {
    update_camera();
	}

  public GameObject get_at_abc(int a, int b, int c) {
    return tiles[a][b][c];
  }

  public GameObject get_at_abc(HexGridCoord hxc) {
    return get_at_abc(hxc.a, hxc.b, hxc.c);
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



  public bool has_adjacency_to_revealed_tile(List<List<int>> lints) {
    foreach (List<int> lint in lints) {
      GameObject gt = tiles[lint[0]][lint[1]][lint[2]];
      if (gt.GetComponent<HexGrid>().is_revealed()) return true;
    }

    return false;
  }

  protected void make_map(int r, ColorizeMethod colorize, string prefab_name) {
    tiles = new Dictionary<int, Dictionary<int, Dictionary<int, GameObject>>>();
    radius_tiles = new Dictionary<int, Dictionary<int, HexadrantTiles>>();

    for (int x = -r; x <= r; x++) {
      for (int y = -r; y <= r; y++) {
        int z = -(x + y);

        if (Mathf.Abs(z) > r) continue;

        if (!tiles.ContainsKey(x)) tiles[x] = new Dictionary<int, Dictionary<int, GameObject>>();
        if (!tiles[x].ContainsKey(y)) tiles[x][y] = new Dictionary<int, GameObject>();
        
        GameObject hex_sprite = (GameObject)Instantiate(Resources.Load(prefab_name));
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



  protected GameObject get_available_tile(int radius, int hexadrant) {
    HexadrantTiles ht = radius_tiles[radius][hexadrant];
    GameObject gt = ht.select_random(randomizer, true);

    return gt;
  }

  protected List<GameObject> get_all_tiles(int radius, int hexadrant, bool mark_used=true) {
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