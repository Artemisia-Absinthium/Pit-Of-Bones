using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    public enum ItemType
    {
        type_range_attack,
        type_potion,
		type_teleportation
    }

	public ItemType type = ItemType.type_range_attack;

	public int range = 0;
	public float castTime = 0f;
	public int damage = 0;
	public int healPoint = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
