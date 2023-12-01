using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using ZIP.DLMS;

namespace Production.Script
{
    public class RootScriptObject
    {

        [JsonProperty("Script")]
        ///
        public Parameterization Script { get; set; }
    }

    public class Parameterization
    {
        List<BaseStep> steps = null;
        public List<BaseStep> baseSteps { get => steps; set => steps = value; }
    }
}
