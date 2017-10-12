using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridTacticalMap : HexGridBase {

	// Use this for initialization
	void Start () {
    base.Start();

    zoom_factor = 4f;
    ColorizeMethod colorizer = colorize;
    make_map(32, colorizer);
	}
	
	// Update is called once per frame
	void Update () {
    base.Update();
	}

  private void colorize(GameObject hex_sprite, int max_radius) {

  }
}
