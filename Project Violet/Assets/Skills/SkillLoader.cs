using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SkillLoader : MonoBehaviour {
  private SkillStruct ss;
  // Use this for initialization
  void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
  
  public void Build() {
    string path = "Assets/Resources/Inits/SkillTiles.ini";
    ss = new SkillStruct();

    List<string> lines = new List<string>(File.ReadAllLines(path));
    foreach (var line in lines) {
      if (line.Trim().Length == 0) continue;
      if (line[0] == '#') continue;
      var commasep = line.Split(',');

      string skill_id = commasep[0].Trim();
      int radius = int.Parse(commasep[1].Trim());
      string skill_name = commasep[2].Trim();
      string bias_method = commasep[4].Trim();
      int hexadrant = int.Parse(commasep[3].Trim());
      List<string> preqs = new List<string>();

      for (int i = 5; i < commasep.Length; i++) {
        preqs.Add(commasep[4].Trim());
      }

      SkillUnit su = new SkillUnit(skill_name, radius, hexadrant, bias_method, preqs);
      ss.Add(skill_id, su);
    }
  }

  public SkillStruct get_struct() {
    return ss;
  }
}