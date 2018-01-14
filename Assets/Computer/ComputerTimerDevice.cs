using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerTimerDevice : MonoBehaviour {

    bool timerTicking;
    public uint InterruptLevel = 3;

    public void StartTimer()
    {
        timerTicking = true;
    }	

    public void StopTimer()
    {
        timerTicking = false;
    }

	void FixedUpdate () {
        if (timerTicking)
        {
            Cpu.SetIRQ(InterruptLevel);
        }
	}
}
