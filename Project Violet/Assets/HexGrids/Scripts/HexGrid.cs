using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
  public HexGridCoord hex_grid_coord;
  public Vector3 abc_coord;
  public Vector3 xyz_coord;
  public int hexadrant;
  public int radius;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public Vector3 spawn(int x, int y, int z) {
    //GameObject hex_visual = (GameObject)Instantiate(Resources.Load("HexGrids/HexVisual"));

    hex_grid_coord = new HexGridCoord(x, y, z);
    Vector3 pl = hex_grid_coord.get_planar_coordinate();
    update_debug_info(pl, x, y, z);
    
    return pl;
  }

  private void update_debug_info(Vector3 pl, int x, int y, int z) {
    abc_coord = new Vector3(x, y, z);
    xyz_coord = pl;
    radius = hex_grid_coord.get_radius();
    hexadrant = hex_grid_coord.get_hexadrant();
  }
}
