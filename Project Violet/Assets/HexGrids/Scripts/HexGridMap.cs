using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridMap : MonoBehaviour {
  
	// Use this for initialization
	void Start () {
    make_map(8);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  private void make_map(int r) {
    for (int x = -r; x <= r; x++) {
      for (int y = -r; y <= r; y++) {
        int z = -(x + y);

        if (Mathf.Abs(z) > r) continue;
        GameObject hex_sprite = (GameObject)Instantiate(Resources.Load("HexSprite"));
        HexGrid hex_grid = hex_sprite.GetComponent<HexGrid>();

        Vector3 pl = hex_grid.spawn(x, y, z);
        hex_sprite.transform.position = pl;
        colorize(hex_sprite);

        if (z > r) break;
      }

     
    }
  }

  private void colorize(GameObject hex_sprite) {
    HexGrid hex_grid = hex_sprite.GetComponent<HexGrid>();
    SpriteRenderer sprite = hex_grid.GetComponent<SpriteRenderer>();

    int hexadrant = hex_grid.hexadrant;

    switch (hexadrant) {
      case 1: sprite.color = new Color(1f, 0f, 0f); break;
      case 2: sprite.color = new Color(0f, 1f, 0f); break;
      case 3: sprite.color = new Color(0f, 0f, 1f); break;
      case 4: sprite.color = new Color(0.5f, 0f, 0f); break;
      case 5: sprite.color = new Color(0f, 0.5f, 0f); break;
      case 6: sprite.color = new Color(0f, 0f, 0.5f); break;
    }
  }
}
