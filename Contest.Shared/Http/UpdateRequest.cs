﻿using Contest.Shared.Enums;
using Contest.Shared.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contest.Shared.Http
{
    public class UpdateRequest
    {
        public string Token { get; set; }
        public long? GenerationsComputed { get; set; }
        public IEnumerable<Coordinate> ResultBoard { get; set; }
    }
}
