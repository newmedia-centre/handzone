/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

#region

using System;
using System.Collections.Generic;
using Schema.Socket.Internals;
using Schema.Socket.Grasshopper;
using Schema.Socket.Realtime;
using Unity.VisualScripting;
using UnityEngine;

#endregion

/// <summary>
/// The RobotManager class is responsible for managing the state and behavior
/// of a robotic system within a Unity environment. It handles the initialization
/// of robot joints, manages the current state of the robot's joints, and
/// facilitates communication with external systems for controlling the robot.
/// The class also listens for real-time data updates and processes inverse
/// kinematics requests to ensure accurate movement and positioning of the robot.
/// </summary>
public class RobotManager : MonoBehaviour
{
    [Header("Robot Joints")] public Transform[] robotJoints;
    public Vector3[] rotationDirection;
    public List<double> qActualJoints;
    private List<Vector3> _initialRotations = new();

    [Header("Prefab")] public GameObject transformGizmoPrefab;

    private List<Outline> _outlines = new();
    private List<BoxCollider> _colliders = new();
    private List<GameObject> _transformGizmos = new();

    // Playback state
    private List<List<double>> _playbackJoints; // radians
    private List<double> _playbackTimes; // seconds between waypoints
    private double _playbackDt; // fallback seconds between waypoints
    private string _playbackUnits = "rad"; // rad|deg
    private bool _isPlaying;
    private bool _hasProgram;
    private Coroutine _playbackRoutine;
    private bool _acceptRealtime = true;
    [Header("Playback")]
    public bool interpolatePlayback = true; // enable smooth interpolation between waypoints

    /// <summary>
    /// Initializes the robot's joints and sets up event listeners for real-time data updates.
    /// This method is called when the script instance is being loaded.
    /// It initializes the joint rotations, colliders, and gizmos, and subscribes
    /// to the session client events for real-time data updates.
    /// </summary>
    private void Start()
    {
        foreach (var robotJoint in robotJoints)
        {
            _initialRotations.Add(robotJoint.localRotation.eulerAngles);

            _colliders.Add(robotJoint.AddComponent<BoxCollider>());
            _transformGizmos.Add(Instantiate(transformGizmoPrefab, robotJoint));
            _transformGizmos[^1].SetActive(false);
        }

        if (SessionClient.Instance == null)
        {
            return;
        }

        SessionClient.Instance.OnRealtimeData += RealtimeDataHandler;
        SessionClient.Instance.OnKinematicCallback += UpdateJointsFromGrabbing;
        SessionClient.Instance.OnGHProgram += OnGrasshopperProgram;
        SessionClient.Instance.OnGHRun += OnGrasshopperRun;

        // Ensure qActualJoints is initialized to the correct size to avoid index errors on first update
        var n = robotJoints != null ? robotJoints.Length : 0;
        if (n > 0)
        {
            if (qActualJoints == null)
            {
                qActualJoints = new List<double>(new double[n]);
            }
            else if (qActualJoints.Count != n)
            {
                qActualJoints.Clear();
                for (int i = 0; i < n; i++) qActualJoints.Add(0);
            }
        }
    }

    /// <summary>
    /// Sets the current joint's angle based on the specified index and angle.
    /// </summary>
    /// <param name="index">The index of the joint to set.</param>
    /// <param name="angle">The angle to set the joint to, in degrees.</param>
    private void SetCurrentJoint(int index, float angle)
    {
        var newJoints = new float[robotJoints.Length];

        robotJoints[index].transform.localRotation = Quaternion.Euler(rotationDirection[index] * angle);

        for (var i = 0; i < robotJoints.Length; i++)
        {
            newJoints[i] = robotJoints[i].transform.localRotation.eulerAngles.magnitude;
            newJoints[i] = FixAngle(newJoints[i], index);
        }
    }

    public void UpdateFromInverseKinematics(List<double> target, Action function)
    {
        var data = new InternalsGetInverseKinIn();
        data.Qnear = qActualJoints;
        data.MaxPositionError = 0.001;
        data.X = target;

        SessionClient.Instance.SendInverseKinematicsRequest(data, function);
    }

    /// <summary>
    /// Updates the robot's joints based on the provided inverse kinematics data
    /// from the Polyscope system.
    /// </summary>
    /// <param name="data">The real-time data containing joint angles.</param>
    public void UpdateJointsFromPolyscope(RealtimeDataOut data)
    {
        if (data == null) return;

        UpdateJoints(data.QActual);
    }

    private void RealtimeDataHandler(RealtimeDataOut data)
    {
        if (!_acceptRealtime) return; // ignore live data during playback
        UpdateJointsFromPolyscope(data);
    }

    /// <summary>
    /// Updates the robot's joints based on the provided inverse kinematics data
    /// from a grabbing action.
    /// </summary>
    /// <param name="data">The inverse kinematics callback data containing joint angles.</param>
    public void UpdateJointsFromGrabbing(InternalsGetInverseKinCallback data)
    {
        if (data == null) return;

        UpdateJoints(data.Ik);
    }

    private void UpdateJoints(List<double> data)
    {
        if (data == null) return;
        var n = Mathf.Min(robotJoints.Length, data.Count);
        for (var i = 0; i < n; i++)
        {
            qActualJoints[i] = data[i];
            var angle = (float)(qActualJoints[i] * Mathf.Rad2Deg);
            // if (i == 0 || i == 4) angle = -angle;
            if (i == 1 || i == 3) angle += 90;

            robotJoints[i].localRotation = Quaternion.Euler(_initialRotations[i] + rotationDirection[i] * angle);
        }
    }

    private void OnGrasshopperProgram(GrasshopperProgramOut data)
    {
        // Cache compact program. Do not auto-start; wait for run=true to keep behavior deterministic.
        _playbackJoints = data.Joints;
        _playbackTimes = data.Times;
        _playbackDt = data.Dt ?? 0.02;
        _playbackUnits = string.IsNullOrEmpty(data.Units) ? "rad" : data.Units;
        _hasProgram = _playbackJoints != null && _playbackJoints.Count > 0;

        // If times are provided as absolute timestamps (same count as joints), convert to per-segment durations
        if (_playbackJoints != null && _playbackTimes != null && _playbackTimes.Count == _playbackJoints.Count)
        {
            var count = _playbackJoints.Count;
            if (count >= 2)
            {
                var durations = new List<double>(count - 1);
                for (int i = 0; i < count - 1; i++)
                {
                    var delta = _playbackTimes[i + 1] - _playbackTimes[i];
                    if (delta <= 0) delta = _playbackDt > 0 ? _playbackDt : 0.02;
                    durations.Add(delta);
                }
                _playbackTimes = durations;
            }
        }

        if (!_hasProgram)
        {
            Debug.LogWarning("Grasshopper program received without compact joints; pausing live briefly to avoid conflicts.");
            if (SessionClient.Instance != null)
            {
                // Briefly disable realtime to avoid fighting live updates while program propagates
                SessionClient.Instance.SetRealtimeEnabled(false);
                SessionClient.Instance.ClearRealtimeQueue();
                // Re-enable after a short delay
                StartCoroutine(ReenableRealtimeSoon());
            }
            return;
        }

        // Auto-start playback immediately on upload for instant feedback
        StopPlayback();

        // Snap to the first waypoint immediately to avoid perceivable delay
        if (_playbackJoints != null && _playbackJoints.Count > 0)
        {
            var first = new List<double>(_playbackJoints[0]);
            var toDeg = _playbackUnits == "deg";
            if (toDeg)
            {
                for (int i = 0; i < first.Count; i++) first[i] = first[i] * Mathf.Deg2Rad;
            }
            UpdateJoints(first);
        }

        // Clear any accumulated realtime frames and start playback loop
        if (SessionClient.Instance != null)
        {
            SessionClient.Instance.ClearRealtimeQueue();
        }

        StartPlayback();
    }

    private System.Collections.IEnumerator ReenableRealtimeSoon()
    {
        yield return new WaitForSeconds(0.25f);
        if (SessionClient.Instance != null)
        {
            SessionClient.Instance.SetRealtimeEnabled(true);
        }
    }

    private void OnGrasshopperRun(bool run)
    {
        if (!_hasProgram)
        {
            Debug.Log("Run signal received but no compact program cached; staying in live mode.");
            return;
        }

        if (run)
        {
            StartPlayback();
        }
        else
        {
            StopPlayback();
        }
    }

    private void StartPlayback()
    {
        if (_isPlaying || !_hasProgram) return;
        _isPlaying = true;
        _acceptRealtime = false; // pause live session updates
        if (SessionClient.Instance != null)
        {
            SessionClient.Instance.SetRealtimeEnabled(false);
        }
        _playbackRoutine = StartCoroutine(PlaybackLoop());
    }

    private void StopPlayback()
    {
        if (!_isPlaying) return;
        _isPlaying = false;
        if (_playbackRoutine != null)
        {
            StopCoroutine(_playbackRoutine);
            _playbackRoutine = null;
        }
        _acceptRealtime = true; // resume live session updates
        if (SessionClient.Instance != null)
        {
            SessionClient.Instance.SetRealtimeEnabled(true);
        }
    }

    private System.Collections.IEnumerator PlaybackLoop()
    {
        if (_playbackJoints == null || _playbackJoints.Count == 0) yield break;

        int count = _playbackJoints.Count;
        bool useTimes = _playbackTimes != null && _playbackTimes.Count == count - 1;
        float defaultDt = (float)(_playbackDt > 0 ? _playbackDt : 0.02f);
        bool inDeg = _playbackUnits == "deg";

        int index = 0;
        while (_isPlaying)
        {
            int next = (index + 1) % count;

            // Prepare endpoints
            var q0 = new List<double>(_playbackJoints[index]);
            var q1 = new List<double>(_playbackJoints[next]);
            if (inDeg)
            {
                for (int i = 0; i < q0.Count; i++) q0[i] = q0[i] * Mathf.Deg2Rad;
                for (int i = 0; i < q1.Count; i++) q1[i] = q1[i] * Mathf.Deg2Rad;
            }

            float segDur = defaultDt;
            if (useTimes)
            {
                var seg = _playbackTimes[index % (count - 1)];
                if (seg > 0) segDur = (float)seg;
            }
            if (segDur < 0.005f) segDur = 0.005f; // tiny clamp

            if (interpolatePlayback && segDur > 0)
            {
                float t = 0f;
                while (t < segDur && _isPlaying)
                {
                    float alpha = segDur > 0 ? t / segDur : 1f;
                    var qi = LerpJoints(q0, q1, alpha);
                    UpdateJoints(qi);
                    yield return null; // next frame
                    t += Time.deltaTime;
                }
            }
            else
            {
                UpdateJoints(q0);
                yield return new WaitForSeconds(segDur);
            }

            // Commit final of the segment to avoid drift
            UpdateJoints(q1);

            index = next;
        }
    }

    private List<double> LerpJoints(List<double> a, List<double> b, float t)
    {
        int n = Mathf.Min(robotJoints.Length, a.Count);
        n = Mathf.Min(n, b.Count);
        var r = new List<double>(n);
        for (int i = 0; i < n; i++)
        {
            r.Add(a[i] * (1.0f - t) + b[i] * t);
        }
        return r;
    }

    /// <summary>
    /// Converts joint angles from the robot's local coordinate system to the
    /// Polyscope coordinate system.
    /// </summary>
    /// <param name="joints">An array of joint angles in degrees.</param>
    /// <returns>An array of joint angles converted to the Polyscope coordinate system.</returns>
    public float[] ToPolyscopeAngles(float[] joints)
    {
        for (var i = 0; i < joints.Length; i++)
        {
            switch (i)
            {
                case 0:
                case 4:
                    joints[i] -= joints[i];
                    break;
                case 1:
                case 3:
                    joints[i] -= 90;
                    break;
            }

            joints[i] = RobotsHelper.WrapAngle(joints[i]);
            joints[i] *= Mathf.Deg2Rad;
        }

        return joints;
    }

    /// <summary>
    /// Fixes the angle of a joint based on its index to ensure it is within
    /// the correct range for the robot's movement.
    /// </summary>
    /// <param name="joint">The angle of the joint to fix.</param>
    /// <param name="index">The index of the joint being fixed.</param>
    /// <returns>The fixed angle of the joint.</returns>
    public float FixAngle(float joint, int index)
    {
        switch (index)
        {
            case 0:
            case 4:
                joint += joint;
                break;
            case 1:
            case 3:
                joint += 90;
                break;
        }

        return joint;
    }
}