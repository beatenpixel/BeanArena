using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Gadget : Equipment {

	public Vector2 hingeJointAnchor;
	public Vector2 hingeJointConnectedAnchor;
	public Vector2 hingeJointLimits = new Vector2(-10, 10);

	private Joint2DCreateResult attachJointResult;

	public void Init() {

	}

	public void InternalStart() {

	}

	public void InternalUpdate() {

	}

	public void InternalLateUpdate() {

	}

	public void InternalFixedUpdate() {

	}

	public override void AttachToHero(HeroBase hero, HeroLimb limb) {
		base.AttachToHero(hero, limb);

		Renderer[] rends = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < rends.Length; i++) {
			rends[i].sortingOrder = limb.rend.baseRendSprite.sortingOrder + 3;
		}

		gameObject.SetLayerRecursively(Game.TeamIDToLayer(hero.info.teamID));

		transform.SetParent(limb.rend.rendT);
		transform.ApplyTransformData(attachTData);

		if (attachType == EquipmentAttachType.Rigidbody) {
			attachJointResult = Joint2DFactory.Create(
				new HingeJoint2DSettings(hingeJointAnchor, hingeJointConnectedAnchor).SetLimits(hingeJointLimits.x, hingeJointLimits.y), rb, limb.rb);
		}
	}

	public override void UnattachFromHero() {
		base.UnattachFromHero();

		if (attachJointResult != null) {
			attachJointResult.DestroyJoint();
		}

		transform.SetParent(null);

		Push();
	}

	protected override void OnDrawGizmosSelected() {
		base.OnDrawGizmosSelected();

		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(transform.TransformPoint(hingeJointAnchor), 0.05f);
	}

	public override string subType {
		get {
			return itemInfo.itemType.ToString();
		}
	}

	public override Type GetPoolObjectType() {
		return typeof(Gadget);
	}

}

public enum GadgetCategory {
	None,
	Melee,
	Gun
}
