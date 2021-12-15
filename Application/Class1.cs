﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class Record
    {
        public int RecordId { get; }
        public string RecordName { get; }
        public string[] Tags { get; set; }
        public DateTime[] Dates { get; set; }
        public string Description { get; set; }
    }
}