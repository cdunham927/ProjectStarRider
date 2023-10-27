using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyUI.Dialogs;

public class DemoDialog : MonoBehaviour
{
    private void Start()
    {
        DialogUI.Instance
        .SetTitle("Notification")
        .SetMessage(" Star Rider is Here")
        .Show ( );
    }
}
