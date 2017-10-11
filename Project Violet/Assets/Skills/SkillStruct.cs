using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStruct {
  private Dictionary<string, SkillUnit> data;

	public SkillStruct() {
    data = new Dictionary<string, SkillUnit>();
  }

  public void Add(string id, SkillUnit su) {
    data.Add(id, su);
  }

  public Dictionary<string, SkillUnit> get_data() {
    return data;
  }
}
