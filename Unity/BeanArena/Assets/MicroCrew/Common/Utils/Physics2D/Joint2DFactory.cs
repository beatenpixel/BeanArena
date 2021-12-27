using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Joint2DFactory {

    public static Joint2DCreateResult Create(Joint2DSettings js, Rigidbody2D targetRb, Rigidbody2D connectedRb) {
        switch (js.type) {
            case Joint2DType.FIXED:
                return CreateFixedJoint2D((FixedJoint2DSettings)js, targetRb, connectedRb);
            case Joint2DType.SPRING:
                return CreateSpringJoint2D((SpringJoint2DSettings)js, targetRb, connectedRb);
            case Joint2DType.HINGE:
                return CreateHingeJoint2D((HingeJoint2DSettings)js, targetRb, connectedRb);
        }

        Debug.LogError("Error! Unknown joint type!");
        return null;
    }

    private static Joint2DCreateResult CreateFixedJoint2D(FixedJoint2DSettings js, Rigidbody2D targetRb, Rigidbody2D connectedRb) {
        Joint2DCreateResult r = new Joint2DCreateResult();
        FixedJoint2D fj = targetRb.gameObject.AddComponent<FixedJoint2D>();
        r.handler = ApplyBaseJointSettings(fj, js, targetRb, connectedRb);

        r.joint = fj;
        r.type = js.type;

        Debug.Log("Create fixed");

        return r;
    }

    private static Joint2DCreateResult CreateSpringJoint2D(SpringJoint2DSettings js, Rigidbody2D targetRb, Rigidbody2D connectedRb) {
        Joint2DCreateResult r = new Joint2DCreateResult();
        SpringJoint2D sj = targetRb.gameObject.AddComponent<SpringJoint2D>();
        r.handler = ApplyBaseJointSettings(sj, js, targetRb, connectedRb);

        r.joint = sj;
        r.type = js.type;

        sj.autoConfigureDistance = js.autoConfigureDst;
        sj.distance = js.distance;
        sj.dampingRatio = js.dampingRatio;
        sj.frequency = js.frequency;

        return r;
    }

    private static Joint2DCreateResult CreateHingeJoint2D(HingeJoint2DSettings js, Rigidbody2D targetRb, Rigidbody2D connectedRb) {
        Joint2DCreateResult r = new Joint2DCreateResult();
        HingeJoint2D hj = targetRb.gameObject.AddComponent<HingeJoint2D>();
        r.handler = ApplyBaseJointSettings(hj, js, targetRb, connectedRb);

        r.joint = hj;
        r.type = js.type;

        hj.useMotor = js.useMotor;
        if (js.useMotor) {
            hj.motor = new JointMotor2D() { motorSpeed = js.motorSpeed, maxMotorTorque = js.maxMotorTorque };
        }

        hj.useLimits = js.useLimits;
        if (js.useLimits) {
            hj.limits = GetHingeJointLimits(js.minMaxLimits, hj.referenceAngle, js.considerReferenceAngle);
        }

        return r;
    }


    private static Joint2DHandler ApplyBaseJointSettings(AnchoredJoint2D joint, Joint2DSettings settings, Rigidbody2D targetRb, Rigidbody2D connectedRb) {
        joint.connectedBody = connectedRb;
        joint.autoConfigureConnectedAnchor = settings.autoConfigureConnectedAnchor;
        joint.anchor = settings.anchor;
        if (!joint.autoConfigureConnectedAnchor) {
            joint.connectedAnchor = settings.connectedAnchor;
        }
        joint.breakForce = settings.breakForce;
        joint.breakTorque = settings.breakTorque;

        if (settings.onBreakCallback != null) {
            Joint2DHandler handler = targetRb.gameObject.AddComponent<Joint2DHandler>();
            handler.Init(settings.onBreakCallback, joint);
            return handler;
        } else {
            return null;
        }
    }

    public static JointAngleLimits2D GetHingeJointLimits(Vector2 minMax, float refAngle, bool considerRefAngle) {
        if (considerRefAngle) {
            return new JointAngleLimits2D() { min = (minMax.x - refAngle) % 360, max = (minMax.y - refAngle) % 360 };
        } else {
            return new JointAngleLimits2D() { min = minMax.x, max = minMax.y };
        }
    }

}

[System.Serializable]
public abstract class Joint2DSettings {
    public Joint2DType type;
    public Vector2 anchor;
    public Vector2 connectedAnchor;
    public bool autoConfigureConnectedAnchor;
    public float breakForce;
    public float breakTorque;

    public Action<Joint2D> onBreakCallback;

    public Joint2DSettings(Vector2 _anchor, Vector2? _connectedAnchor, float _breakForce = 10000, float _breakTorque = 10000) {
        anchor = _anchor;
        connectedAnchor = _connectedAnchor ?? Vector2.zero;
        autoConfigureConnectedAnchor = _connectedAnchor == null;
        breakForce = _breakForce;
        breakTorque = _breakTorque;
    }

    public Joint2DSettings OnBreak(Action<Joint2D> _onBreakCallback) {
        onBreakCallback = _onBreakCallback;
        return this;
    }

}


public class HingeJoint2DSettings : Joint2DSettings {
    public bool useMotor;
    public float motorSpeed;
    public float maxMotorTorque;
    public bool useLimits;
    public Vector2 minMaxLimits;
    public bool considerReferenceAngle;

    public HingeJoint2DSettings(Vector2 _anchor, Vector2? _connectedAnchor, float _breakForce = 10000, float _breakTorque = 10000) : base(_anchor, _connectedAnchor, _breakForce, _breakTorque) {
        type = Joint2DType.HINGE;
    }

    public HingeJoint2DSettings SetLimits(float min, float max, bool _considerReferenceAngle = true) {
        useLimits = true;
        minMaxLimits = new Vector2(min, max);
        considerReferenceAngle = _considerReferenceAngle;
        return this;
    }

    public HingeJoint2DSettings SetLimits(HingeJointLimits2D limit) {
        useLimits = limit.useLimit;
        minMaxLimits = limit.minMax;
        considerReferenceAngle = limit.considerReferenceAngle;
        return this;
    }

    public HingeJoint2DSettings SetMotor(float speed, float maxForce) {
        useMotor = true;
        motorSpeed = speed;
        maxMotorTorque = maxForce;
        return this;
    }

}

public class SpringJoint2DSettings : Joint2DSettings {
    public bool autoConfigureDst = false;
    public float distance;
    public float dampingRatio;
    public float frequency;

    public SpringJoint2DSettings(Vector2 _anchor, Vector2? _connectedAnchor, float _breakForce = 10000, float _breakTorque = 10000) : base(_anchor, _connectedAnchor, _breakForce, _breakTorque) {
        type = Joint2DType.SPRING;
    }

    public SpringJoint2DSettings SetSpring(float dst, float freq, float damp, bool _autoConfigureDst = false) {
        autoConfigureDst = _autoConfigureDst;
        distance = dst;
        frequency = freq;
        dampingRatio = damp;
        return this;
    }
}

public class FixedJoint2DSettings : Joint2DSettings {
    public bool autoConfigureDst = false;
    public float distance;
    public float dampingRatio;
    public float frequency;
    public float refAngle;

    public FixedJoint2DSettings(Vector2 _anchor, Vector2? _connectedAnchor, float _refAngle, float _breakForce = 10000, float _breakTorque = 10000) : base(_anchor, _connectedAnchor, _breakForce, _breakTorque) {
        type = Joint2DType.FIXED;
        refAngle = _refAngle;
    }

}

[System.Serializable]
public struct HingeJointLimits2D {
    public bool useLimit;
    public bool considerReferenceAngle;
    public Vector2 minMax;
}

public class Joint2DCreateResult {
    public Joint2D joint;
    public Joint2DType type;
    public Joint2DHandler handler;

    public void DestroyJoint() {
        if (handler != null) {
            MonoBehaviour.Destroy(handler);
        }

        if (joint != null) {
            MonoBehaviour.Destroy(joint);
        }
    }
}

public enum Joint2DType {
    NONE,
    FIXED,
    HINGE,
    SPRING
}