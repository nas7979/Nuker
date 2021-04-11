using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public enum JoyStickState
    {
        NORMAL = 0,
        RED,
        BLUE
    }

    public class Enums<T>
    {
        public static int GetEnumListCount()
            => Enum.GetNames(typeof(T)).Length;
    }
}