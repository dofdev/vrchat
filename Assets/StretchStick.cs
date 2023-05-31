using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StretchStick : UdonSharpBehaviour {
	[SerializeField] VRC_Pickup vrc_pickup;
	
	[Header("input")]
	[SerializeField] bool righty = false;
	Vector3[] hands = new Vector3[2];

	[SerializeField] bool held = false;
	public override void OnPickup() => held = true;
	public override void OnDrop()   => held = false;

	[SerializeField] bool interact = false;
	public override void OnPickupUseDown() => interact = true;
	public override void OnPickupUseUp() => interact = false;

	[Header("data")]
	Vector3 cursor = Vector3.zero;

	[SerializeField] bool stuck = false;
	[SerializeField] Vector3 stuck_pos = Vector3.zero;

	VRCPlayerApi playerApi;
	void Start() {
		playerApi = Networking.LocalPlayer;
	}

	void Update() {
		hands[0] = playerApi.GetBonePosition(HumanBodyBones.LeftHand);
		hands[1] = playerApi.GetBonePosition(HumanBodyBones.RightHand);
		
		if (held) {
			righty = vrc_pickup.currentHand == VRC_Pickup.PickupHand.Right;

			Vector3 to   = hands[righty ? 1 : 0];
			Vector3 from = hands[righty ? 0 : 1];

			Vector3 delta = to - from;
			float mag = delta.magnitude;
			float stretch = Mathf.Max(mag - deadzone, 0);

			Vector3 dir = delta.normalized;
			cursor = to + dir * stretch * strength;

			// spring
			if (cursor.y < 0) {
				// doing a raycast back to the entry point would give us a better vector?
				if (!stuck) {
					RaycastHit hit;
					if (Physics.Raycast(
						cursor, Custom.Direction(old_cursor, cursor), out hit, 
						1024f, LayerMask.GetMask("Default")
					)) {
						stuck_pos = hit.point;
						stuck = true;
						Debug.Log("stuck");
					}
				}
				
				if (stuck) {
					Vector3 vel = playerApi.GetVelocity();
					playerApi.SetVelocity(vel + (stuck_pos - cursor));
					// oldPos = back;
					// stuck = false;
				}
			} else {
				stuck = false;
			}
		}


		// draw
		gameObject.DrawIn("cursor", cursor, Quaternion.identity, 1f, held);
		gameObject.DrawIn("stuck", stuck_pos, Quaternion.identity, 1f, held && stuck);

		//
		old_cursor = cursor;
	}
	Vector3 old_cursor = Vector3.zero;

	[Header("design")]
	[SerializeField] float deadzone = 0.1f;
	[SerializeField] float strength = 3f;
}