﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Library_benchmark.Models
{
    public class Parametros
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int Sheets { get; set; }
        public bool Design { get; set; }
        public bool Resource { get; set; }
        public int Iteraciones { get; set; }
    }
}