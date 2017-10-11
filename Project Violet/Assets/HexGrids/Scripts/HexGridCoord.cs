using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridCoord {
  const float x_scaler = 1f;
  const float y_scaler = 0.8f;
  private int x, y, z;
	
  public HexGridCoord() {
    x = y = z = 0;
  }

  public HexGridCoord(int _x, int _y, int _z) {
    set_position(_x, _y, _z);
  }

  public void set_position(int _x, int _y, int _z) {
    x = _x;
    y = _y;
    z = _z;
  }

  public Vector3 get_planar_coordinate() {
    return new Vector3(x_scaler * (float) (y + 0.5f * z), y_scaler * (float) (x + y), 0);
  }

  public void set_relative(HexDirection.Direction dir, int multiples = 1) {
    int x_translate = 0;
    int y_translate = 0;
    int z_translate = 0;

    switch(dir) {
      case HexDirection.Direction.E: x_translate = -1; y_translate = +1; break;
      case HexDirection.Direction.W: x_translate = +1; y_translate = -1; break;
      case HexDirection.Direction.NE: y_translate = +1; z_translate = -1; break;
      case HexDirection.Direction.SW: y_translate = +1; z_translate = -1; break;
      case HexDirection.Direction.NW: x_translate = +1; z_translate = -1; break;
      case HexDirection.Direction.SE: x_translate = -1; z_translate = +1; break;
    }

    x = x_translate * multiples;
    y = y_translate * multiples;
    z = z_translate * multiples;
  }
}
