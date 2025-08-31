using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRiderController : PlayerController
{
    //Aniamtion State  make sure string match name of animations
    const string StarRiderIdle = "StarRiderIdle";
    const string StarRiderShip_BarrelRoll = "StarRiderShip|BarrelRoll";
    const string StarRiderShip_Go_Fast = "StarRiderShip|Go_Fast";
    const string StarRiderShip_Go_Slow = "StarRiderShip|Go_Slow";
    const string StarRiderShip_Spin = "StarRiderShip|Spin";

    public override void Special()
    {

    }


}
