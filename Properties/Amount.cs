using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amount : MonoBehaviour {

	public float amountOf;
    public bool infinite;    
	public bool random;
    [MinMaxRange(0,1000)]
    [SerializeField]
    MinMaxRange range;
    public float limit;

	void Start(){
		if (random) {
          amountOf = range.GetRandomValue();
		}

	}


    void Update(){
        if(infinite)
            amountOf = 100;

        if (limit > 0)
            if (amountOf > limit)
                amountOf = limit;
    }


    
}
