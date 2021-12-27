using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joint2DHandler : MonoBehaviour {

    public Joint2D observableJoint;

    public event Action<Joint2D> onBreakCallback;
    private Timer checkNullConnectedBodyTimer;

    public void Init(Action<Joint2D> _onBreakCallback, Joint2D _observableJoint) {
        onBreakCallback = _onBreakCallback;
        observableJoint = _observableJoint;
        checkNullConnectedBodyTimer = new Timer(0.5f, false);
    }

    private void Update() {
        if (checkNullConnectedBodyTimer) {
            checkNullConnectedBodyTimer.AddFromNow();

            if (observableJoint.connectedBody == null) {
                OnBreak();
            }
        }
    }

    private void OnJointBreak2D(Joint2D joint) {
        if (joint == observableJoint) {
            OnBreak();
        }
    }

    private void OnBreak() {
        onBreakCallback?.Invoke(observableJoint);
        Destroy(this);
    }

}
