using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
  public HexGridCoord hex_grid_coord;
  public Vector3 abc_coord;
  public Vector3 xyz_coord;
  public int hexadrant;
  public int radius;
  const int dihex_radius_cutoff = 14;

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
    
    return pl;
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
