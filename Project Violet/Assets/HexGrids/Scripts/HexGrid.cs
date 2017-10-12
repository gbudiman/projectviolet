using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
  public HexGridCoord hex_grid_coord;
  public Vector3 abc_coord;
  public Vector3 xyz_coord;
  public int hexadrant;
  public int radius;
  private bool revealed;
  public bool profession_border = false;
  const int dihex_radius_cutoff = 14;
  HexGridMap hex_grid_map = null;

	// Use this for initialization
	void Start () {
    
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public Vector3 spawn(int x, int y, int z) {

    hex_grid_coord = new HexGridCoord(x, y, z);
    Vector3 pl = hex_grid_coord.get_planar_coordinate();
    update_debug_info(pl, x, y, z);
    revealed = false;
    
    return pl;
  }

  private List<List<int>> get_adjacents() {
    int a = (int) abc_coord.x;
    int b = (int) abc_coord.y;
    int c = (int) abc_coord.z;
    List<List<int>> master = new List<List<int>>();

    master.Add(new List<int>() { a + 1, b    , c - 1 });
    master.Add(new List<int>() { a    , b + 1, c - 1 });
    master.Add(new List<int>() { a - 1, b + 1, c     });
    master.Add(new List<int>() { a - 1, b    , c + 1 });
    master.Add(new List<int>() { a    , b - 1, c + 1 });
    master.Add(new List<int>() { a + 1, b - 1, c     });

    return master;
  }

  public void reveal(bool forced=false) {
    if (hex_grid_map == null) hex_grid_map = FindObjectOfType<HexGridMap>();

    if (!revealed) {
      if (forced || hex_grid_map.has_adjacency_to_revealed_tile(get_adjacents())) {
        GetComponentInChildren<MeshRenderer>().enabled = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float hue, saturation, lightness;
        Color.RGBToHSV(sr.color, out hue, out saturation, out lightness);

        sr.color = Color.HSVToRGB(hue, saturation, lightness + 0.5f);

        revealed = true;
      }
    }
  }

  public void debug_reveal(bool state) {
    if (state) {
      GetComponentInChildren<MeshRenderer>().enabled = state;
    } else {
      if (!revealed) {
        GetComponentInChildren<MeshRenderer>().enabled = state;
      }
    }
  }

  public bool is_revealed() {
    return revealed;
  }

  private void OnMouseDown() {
    reveal();
  }

  private void update_debug_info(Vector3 pl, int x, int y, int z) {
    abc_coord = new Vector3(x, y, z);
    xyz_coord = pl;
    radius = hex_grid_coord.get_radius();

    if (radius < dihex_radius_cutoff) {
      hexadrant = hex_grid_coord.get_hexadrant();
    } else {
      hexadrant = hex_grid_coord.get_dihexadrant();
    }
  }

  public void set_color(TileType type, int max_radius) {
    float hue = 0f;
    float saturation = 0f;
    float lightness = 0f;
    float alpha = 0.75f;
    SpriteRenderer sr = GetComponent<SpriteRenderer>();
    TextMesh tm = GetComponentInChildren<TextMesh>();

    if (hexadrant == -1) {
      lightness = (float)radius / (float)max_radius * 0.25f + 0.5f;
      alpha = 1.0f;
    } else {
      if (type == TileType.preset_skill) {
        hue = (hexadrant - 1) * 60f / 360f;
        //lightness = (float)radius / (float)max_radius * 0.25f;
        //saturation = 0.5f;
        lightness = 0.5f;
        saturation = 0.75f;
      } else if (type == TileType.fixed_stat) {
        hue = 0f;
        saturation = 0f;
        lightness = 0.1f;
      } else if (type == TileType.any_stat) {
        hue = 0f;
        saturation = 0f;
        lightness = 0.25f;
        //tm.color = new Color(0f, 0f, 0f);
      } else {
        lightness = 1f;
        tm.color = new Color(0f, 0f, 0f);
      }
    }

    Color c = Color.HSVToRGB(hue, saturation, lightness);
    sr.color = new Color(c.r, c.g, c.b, alpha);
  }
}
