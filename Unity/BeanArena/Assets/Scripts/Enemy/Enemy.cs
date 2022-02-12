using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy {

    public HeroBase hero { get; private set; }

    private Timer changeDirTimer;
    private Vector2 moveInput;
    private Vector2 armInput;

    private Timer shootTimer;

    public void Init() {
        changeDirTimer = new Timer(MRandom.Range(0.5f, 3f));
        shootTimer = new Timer(MRandom.Range(0.75f, 1.5f));

        RandomizeInput();
    }

    public void InternalUpdate() {
        if(hero != null) {
            if(!GameMode.current.heroesInputAllowed) {
                hero.MoveInput(Vector2.zero);
                return;
            }

            if (changeDirTimer) {
                changeDirTimer.AddFromNow(MRandom.Range(0.5f, 3f));
                RandomizeInput();
            }

            hero.MoveInput(moveInput);
            //hero.ArmInput(armInput);

            if(shootTimer) {
                shootTimer.AddFromNow(MRandom.Range(0.75f, 1.5f));
                hero.ButtonInput(new ButtonInputEventData(0, 0.5f));
                hero.ButtonInput(new ButtonInputEventData(1, 0.5f));
            }            
        }
    }

    private void RandomizeInput() {
        //moveInput = new Vector2(MRandom.Range(-0.6f, 0.6f), 1f);
        //armInput = Quaternion.Euler(0, 0, MRandom.Range(0, 360f)) * Vector3.right;
    }

    public void AssignHero(HeroBase _hero) {
        hero = _hero;
    }

}
