using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy {

    public Hero hero { get; private set; }

    private Timer changeDirTimer;
    private Vector2 moveInput;
    private Vector2 armInput;

    public void Init() {
        changeDirTimer = new Timer(MRandom.Range(0.5f, 3f));
        RandomizeInput();
    }

    public void InternalUpdate() {
        if(hero != null) {
            if(changeDirTimer) {
                changeDirTimer.AddFromNow(MRandom.Range(0.5f, 3f));
                RandomizeInput();
            }

            hero.MoveInput(moveInput);
            //hero.ArmInput(armInput);

            hero.ButtonInput(new ButtonInputEventData(0, 0.5f));
            hero.ButtonInput(new ButtonInputEventData(1, 0.5f));
        }
    }

    private void RandomizeInput() {
        moveInput = new Vector2(MRandom.Range(-0.6f, 0.6f), 1f);
        armInput = Quaternion.Euler(0, 0, MRandom.Range(0, 360f)) * Vector3.right;
    }

    public void AssignHero(Hero _hero) {
        hero = _hero;
    }

}
