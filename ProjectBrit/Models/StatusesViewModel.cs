using System;
using ProjectBrit.Data;
using System.Collections.Generic;

namespace ProjectBrit.Models
{
    public class StatusesViewModel
    {        
        public StatusesViewModel(List<Status> statuses)
        {
            this.Statuses = statuses;
        }

        public List<Status> Statuses { get; set; }

    }
}