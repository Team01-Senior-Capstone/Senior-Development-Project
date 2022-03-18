using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisconnetException : Exception
{
    public DisconnetException() : base() { }
    public DisconnetException(string message) : base(message) { }
    public DisconnetException(string message, Exception inner) : base(message, inner) { }
}
