using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common;

public class Mono : UdonSharpBehaviour {
	VRCPlayerApi playerApi;
	void Start() {
		playerApi = Networking.LocalPlayer;
	}

	void Update() {
		gameObject.DrawIn("ship", new Vector3(Mathf.Sin(Time.time) * 0.333f, 0.666f, 0));
	}
}

public static class Custom {
	public static Vector3 Direction(Vector3 to, Vector3 from) {
		return (to - from).normalized;
	}

	public static void DrawIn(this GameObject of, string name, Vector3 pos, Quaternion rot = new Quaternion(), float scale = 1f, bool active = true) {
		Transform t = of.transform.Find(name);
		if (t == null) {
			Debug.LogError("no gameobject found with name: " + name);
			return;
		}
		t.position = pos;
		t.rotation = rot;
		t.localScale = Vector3.one * scale;
		t.gameObject.SetActive(active);
	}
}


/*
	dofdev is a hub world
	*it's also mono
	meaning projects start there first

	nest egg
		this would actually work out great
		just need to figure out how the hazards/enemies work
		but you could chill with x# of friends and just play
		balancing risk/reward

		new mechanic being that you can destroy/free an item
		by dropping/throwing it over the edge or something
		releasing the enemy
		or spawning a new one

		that way you can play for longer and can cycle items

		the cool bit is arranging your space


		this was orginally made as a cheeky abstract arcade game
		so im already watching it fall apart in my head
		when i look to introduce things that it needs to best vrchat

		for example if you want the items to be more than random prop

	color cube
		put shapes in *fingers and more
		normal control(roll)
		fancier color blending


	stretch silo
		is the chill one to start out with
		as co-op fixes the range of vision issue
		and vs is fun too *like battleship
	
	you are based somewhere in the city/county
	and you can move strategically in order to minimize risk
		as a single unit or (radar | command sim | drone | citizens | power station |  ||...)

		I want this too have more emotional impact, compared to last time
		doesn't necassarily mean it needs to be complicated and realistic

		but we can use some real life elements as jumping off points


		more importantly howevew is to fit the medium(vrchat)
		arcade games were designed for arcades

		chat games were designed for vrchat

		meaning the game should act more as a social lubricant
		rather than a frantic self centered experience

		meaning score chasing and quarter munching are out

		providing it's own unique platform for social interaction



	bending
		almost all the bending stuff makes use of fbt
		so just embrace that?

		even with air bending, when you move your body light as a feather
		you are *resonating with the air around you

		the fucking hard part is the minds eye stuff
		how do you make that happen in vr with current tech?

	start with air
		it's *always right around you
		the visuals can be subtle

		staff seems important

		just have a gameobject pool of air that keeps respawning back around you
		
*/