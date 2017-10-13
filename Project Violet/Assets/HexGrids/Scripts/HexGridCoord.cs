using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridCoord {
  const float x_scaler = 1f;
  const float y_scaler = 0.85f;
  public int a, b, c;
	
  public HexGridCoord() {
    a = b = c = 0;
  }

  public HexGridCoord(int _a, int _b, int _c) {
    set_position(_a, _b, _c);
  }

  public void set_position(int _a, int _b, int _c) {
    a = _a;
    b = _b;
    c = _c;
  }

  public bool equals(HexGridCoord other) {
    return (other.a == a && other.b == b && other.c == c);
  }

  public Vector3 get_planar_coordinate() {
    return new Vector3(x_scaler * (float) (b + 0.5f * c), y_scaler * (float) (a + b), 0);
  }

  public void set_relative(HexDirection.Direction dir, int multiples = 1) {
    int a_translate = 0;
    int b_translate = 0;
    int c_translate = 0;

    switch(dir) {
      case HexDirection.Direction.E: a_translate = -1; b_translate = +1; break;
      case HexDirection.Direction.W: a_translate = +1; b_translate = -1; break;
      case HexDirection.Direction.NE: b_translate = +1; c_translate = -1; break;
      case HexDirection.Direction.SW: b_translate = +1; c_translate = -1; break;
      case HexDirection.Direction.NW: a_translate = +1; c_translate = -1; break;
      case HexDirection.Direction.SE: a_translate = -1; c_translate = +1; break;
    }

    a = a_translate * multiples;
    b = b_translate * multiples;
    c = c_translate * multiples;
  }

  public List<HexGridCoord> get_adjacent_grids() {
    return new List<HexGridCoord>() {
      new HexGridCoord(a + 1, b    , c - 1 ),
      new HexGridCoord(a,     b + 1, c - 1 ),
      new HexGridCoord(a - 1, b + 1, c ),
      new HexGridCoord(a - 1, b    , c + 1 ),
      new HexGridCoord(a    , b - 1, c + 1 ),
      new HexGridCoord(a + 1, b - 1, c     )
  };

 
  }

  public int get_hexadrant() {
    int r = get_radius();

    if (r == 0) {

    } else if (0 < a && a < r) {
      // Hxd I or V
      if (b == -r) { return 5; }
      return 1;
    } else if (-r < a && a < 0) {
      // Hxd II or Iv
      if (b == r) { return 2; }
      return 4;
    } else if (0 < b && b < r) {
      return 3;
    } else if (-r < b && b < 0) {
      return 6;
    }

    return -1;
  }

  public int get_dihexadrant() {
    int hexadrant = get_hexadrant();

    switch(hexadrant) {
      case 1:
        if (a > b) return 11;
        else if (a < b) return 12; break;
      case 2:
        if (a > c) return 21;
        else if (a < c) return 22; break;
      case 3:
        if (b > c) return 31;
        else if (b < c) return 32; break;
      case 4:
        if (a < b) return 41;
        else if (a > b) return 42; break;
      case 5:
        if (a < c) return 51;
        else if (a > c) return 52; break;
      case 6:
        if (b < c) return 61;
        else if (b > c) return 62; break;
    }

    return -1;
  }

  public int get_radius() {
    return (Mathf.Abs(a) + Mathf.Abs(b) + Mathf.Abs(c)) / 2;
  }
}
