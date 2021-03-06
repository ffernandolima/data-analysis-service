﻿using System.Globalization;
using System.Threading;

namespace DataAnalysis.Framework.Initialization
{
    public static class CultureConfigurator
    {
        public static void Configure()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        }
    }
}
