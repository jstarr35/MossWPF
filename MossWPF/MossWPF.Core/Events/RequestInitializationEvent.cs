﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MossWPF.Core.Events
{
    public class RequestInitializationEvent : PubSubEvent<Dictionary<string,string>>
    {
    }
}
