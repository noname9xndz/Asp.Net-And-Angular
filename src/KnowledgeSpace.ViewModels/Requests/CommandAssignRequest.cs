using System;
using System.Collections.Generic;
using System.Text;

namespace KnowledgeSpace.ViewModels.Requests
{
    public class CommandAssignRequest
    {
        public string[] CommandIds { get; set; }

        public bool AddToAllFunctions { get; set; }
    }
}