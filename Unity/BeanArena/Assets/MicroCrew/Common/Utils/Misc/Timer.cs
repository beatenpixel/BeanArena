using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer {

    private float nextTime;
    private float interval;

    private int nextFrame;
    private int frameInterval;

    private bool isFrameBased;
    private bool realtime;

    public Timer(float _interval, bool _realtime = false, bool waitOnCreate = true) {
        realtime = _realtime;
        isFrameBased = false;
        interval = _interval;
        if (realtime) {
            if (waitOnCreate) {
                nextTime = Time.realtimeSinceStartup + interval;
            } else {
                nextTime = Time.realtimeSinceStartup;
            }
        } else {
            if (waitOnCreate) {
                nextTime = Time.time + interval;
            } else {
                nextTime = Time.time;
            }
        }
    }

    public Timer(int _frameInterval) {
        isFrameBased = true;
        frameInterval = _frameInterval;
        nextFrame = Time.frameCount + frameInterval;
    }

    public void AddFromNow() {
        if (isFrameBased) {
            nextFrame = Time.frameCount + frameInterval;
        } else {
            if (realtime) {
                nextTime = Time.realtimeSinceStartup + interval;
            } else {
                nextTime = Time.time + interval;
            }
        }
    }

    public void AddFromNow(float amount) {
        if (isFrameBased) {
            Debug.LogError("Can't add float to framed timer!");
            nextFrame = Time.frameCount + frameInterval;
        } else {
            if (realtime) {
                nextTime = Time.realtimeSinceStartup + amount;
            } else {
                nextTime = Time.time + amount;
            }
        }
    }

    public static implicit operator bool(Timer t) {
        if (t.isFrameBased) {
            return Time.frameCount > t.nextFrame;
        } else {
            if (t.realtime) {
                return Time.realtimeSinceStartup > t.nextTime;
            } else {
                return Time.time > t.nextTime;
            }
        }
    }

}