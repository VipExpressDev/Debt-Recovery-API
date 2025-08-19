using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebtRecoveryPlatform.Models.NonPersistent
{
    public class ActionLogger
    {
        public string Action { get; set; }
        public string View { get; set; }
        public string ActionedBy { get; set; }

        public ActionLogger()
        {

        }

        public ActionLogger(string _action, string _view, string _actionedBy)
        {
            Action = _action;
            View = _view;
            ActionedBy = _actionedBy;
        }
    }
}
