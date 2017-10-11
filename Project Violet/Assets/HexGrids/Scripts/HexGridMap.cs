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

        if (z > r) break;
      }

     
    }
  }
}
