using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class StretchStick : UdonSharpBehaviour {
	[SerializeField] VRC_Pickup vrc_pickup;
	
	[Header("input")]
	[SerializeField] bool righty = false;
	Vector3[] hands = new Vector3[2];

	[SerializeField, UdonSynced] bool held = false;
	public override void OnPickup() => held = true;
	public override void OnDrop()   => held = false;

	[SerializeField] bool interact = false;
	public override void OnPickupUseDown() => interact = true;
	public override void OnPickupUseUp() => interact = false;

	[Header("data")]
	[UdonSynced(UdonSyncMode.Linear)] Vector3 cursor = Vector3.zero;
	Quaternion rot_offset = Quaternion.identity;

	[SerializeField, UdonSynced] bool stuck = false;
	[UdonSynced] Vector3 stuck_pos = Vector3.zero;

	VRCPlayerApi player_api;
	void Start() {
		player_api = Networking.LocalPlayer;
	}

	void Update() {
		// if (player_api != vrc_pickup.currentPlayer) return;
		
		if (held && player_api == vrc_pickup.currentPlayer) {
			righty = vrc_pickup.currentHand == VRC_Pickup.PickupHand.Right;
			hands[0] = player_api.GetBonePosition(HumanBodyBones.LeftHand);
			hands[1] = player_api.GetBonePosition(HumanBodyBones.RightHand);
			Vector3 to   = hands[righty ? 1 : 0];
			Vector3 from = hands[righty ? 0 : 1];
			Quaternion rot = righty ? 
				player_api.GetBoneRotation(HumanBodyBones.RightHand) : 
				player_api.GetBoneRotation(HumanBodyBones.LeftHand);

			Vector3 delta = to - from;
			float mag = delta.magnitude;
			float stretch = Mathf.Max(mag - deadzone, 0);
			
			Vector3 dir = delta.normalized;

			if (interact) {
				// cursor is now static
				// so we can adjust the strength and rotation offset relative to the cursor
				// on release it should be in the same spot as before
				// meaning the strength and rot_offset should be adjusted to meet that
				float cursor_dist = Vector3.Distance(to, cursor);
				strength = Mathf.Clamp(cursor_dist / stretch, 1f, 15f);
				Quaternion from_rot = rot;
				Quaternion to_rot  = Quaternion.LookRotation(Custom.Direction(cursor, to), Vector3.up);
				rot_offset = Quaternion.Inverse(from_rot) * to_rot;

			} else {
				cursor = to + rot * rot_offset * Vector3.forward * stretch * strength;

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
						Vector3 vel = player_api.GetVelocity();
						player_api.SetVelocity(vel + (stuck_pos - cursor));
					}
				} else {
					stuck = false;
				}
			}

			reset_countdown = 6f;
		} else {
			// reset
			interact = false; // just in case

			strength = 3f;
			rot_offset = Quaternion.identity;
			if (reset_countdown > 0) {
				reset_countdown -= Time.deltaTime;
				if (reset_countdown <= 0) {
					transform.rotation = Quaternion.identity;
					transform.position = new Vector3(-2, 0.64f, 0);
				}
			}
		}


		// draw
		gameObject.DrawIn("cursor", cursor, Quaternion.identity, interact ? 2:1, held);
		gameObject.DrawIn("stuck", stuck_pos, Quaternion.identity, 1, held && stuck);

		//
		old_cursor = cursor;
	}
	Vector3 old_cursor = Vector3.zero;
	float reset_countdown = 0f;

	[Header("design")]
	[SerializeField] float deadzone = 0.1f;
	[SerializeField] float strength = 3f;
}