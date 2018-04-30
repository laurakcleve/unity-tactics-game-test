using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {

    public static PlayerInactive playerInactive = new PlayerInactive();
    public static PlayerSelectTurnAction playerSelectTurnAction = new PlayerSelectTurnAction();
    public static PlayerSelectDestination playerSelectDestination = new PlayerSelectDestination();
    public static PlayerConfirmDestination playerConfirmDestination = new PlayerConfirmDestination();
    public static PlayerSelectTarget playerSelectTarget = new PlayerSelectTarget();
    public static PlayerMenus playerMenus = new PlayerMenus();

    public virtual void Enter(AIUnit unit) { }
    public virtual void Enter(PlayerUnit unit) { }

    public virtual void Exit(AIUnit unit) { }
    public virtual void Exit(PlayerUnit unit) { }

}
