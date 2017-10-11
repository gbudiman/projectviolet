using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnit {
  public string skill_name;
  public int radius;
  public int hexadrant;
  public string bias_method;
  public List<string> preqs;

  public SkillUnit(string _skill_name, int _radius, int _hexadrant, string _bias_method, List<string> _preqs) {
    skill_name = _skill_name;
    radius = _radius;
    hexadrant = _hexadrant;
    bias_method = _bias_method;
    preqs = _preqs;
  }

}
