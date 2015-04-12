using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord
{
    public class History
    {
        private List<Action> _actions = new List<Action>();
        //public List<Action> Actions { get; set; }

        public History()
        {
            //Actions = new List<Action>();
        }

        public void AddAction(Action action)
        {
            _actions.Add(action);
        }

        public void RevertLastAction()
        {
            Action lastAction = _actions[_actions.Count - 1];
            
        }

        public void Clear()
        {
            _actions.Clear();
        }
    }
}
