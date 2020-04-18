﻿using System;
using System.Collections.Generic;

namespace TurnipTrackerApi.Models.Boards
{
    public class BoardModel
    {
        public string UrlName { get; set; }

        public string DisplayName { get; set; }

        public IEnumerable<DateTime> Weeks { get; set; }
    }
}