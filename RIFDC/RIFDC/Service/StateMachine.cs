using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ObjectParameterEngine;


namespace StateMachineNamespace
{
    //это класс, который управляет действиями, которые делает объект при переходе в некое состояние
    //объект, переходящий из состояния в состояние, должен быть наследником этого класса
    public class StateMachine
    {
        public  delegate void actionDelegate();

        private int _currentState;
        public int currentState 
        { 
            get 
            { 
                return _currentState; 
            } 
            set
            {
                performStateActions(_currentState, ActionOnEnterOrLeaveEnum.Leave); //сделать действия на выходе из старого состояния
                _currentState = value; //присвоить новое 
                performStateActions(_currentState, ActionOnEnterOrLeaveEnum.Enter); //сделать действия на входе в новое состояние
            }
        }

        private List<StateAction> stateActions = new List<StateAction>();

        //выполнить действия, полагающиеся на входе / выходе из конкретного состояния
        private void performStateActions(int stateId, ActionOnEnterOrLeaveEnum enterOrLeave)
        {
            bool visible;
            foreach (StateAction sa in stateActions)
            {
                if (sa.stateId== stateId && sa.enterOrLeave== enterOrLeave)
                {
                    //нашли нужное нам действие
                    switch (sa.actionType)
                    {
                        case StateActionTypeEnum.Show:
                        case StateActionTypeEnum.Hide:
                            visible = sa.actionType == StateActionTypeEnum.Show;
                            
                            foreach (Control ctl in sa.targetControls)
                            {
                                ctl.Visible = visible;
                            }
                            break;

                        case StateActionTypeEnum.SetProperty:
                            foreach (Control ctl in sa.targetControls)
                            {
                                ObjectParameters.setObjectParameter(ctl, sa.targetParameter, sa.targetParameterValue);
                            }
                            break;

                        case StateActionTypeEnum.ExecDelegate:
                            sa.doAction();
                            break;

                    }
                }
            }
        }


        //показать контролы
        public void addShowAction(int stateId, ActionOnEnterOrLeaveEnum enterOrLeave, List<Control> targetControls)
        {
            StateAction c = new StateAction();
            c.stateId = stateId;
            c.enterOrLeave = enterOrLeave;
            c.actionType = StateActionTypeEnum.Show;
            c.targetControls = targetControls;
            stateActions.Add(c);
        }
        public void addHideAction(int stateId, ActionOnEnterOrLeaveEnum enterOrLeave, List<Control> targetControls)
        {
            StateAction c = new StateAction();
            c.stateId = stateId;
            c.enterOrLeave = enterOrLeave;
            c.actionType = StateActionTypeEnum.Hide;
            c.targetControls = targetControls;
            stateActions.Add(c);
        }

        public void addSetPropertyAction(int stateId, ActionOnEnterOrLeaveEnum enterOrLeave, string targetParameter, object targetParameterValue, List<Control> targetControls)
        {
            StateAction c = new StateAction();
            c.stateId = stateId;
            c.enterOrLeave = enterOrLeave;
            c.actionType = StateActionTypeEnum.SetProperty;
            c.targetControls = targetControls;
            c.targetParameter = targetParameter;
            c.targetParameterValue = targetParameterValue;
            stateActions.Add(c);
        }
        public void addDelegateAction(int stateId, ActionOnEnterOrLeaveEnum enterOrLeave, actionDelegate dlg)
        {
            StateAction c = new StateAction();
            c.stateId = stateId;
            c.enterOrLeave = enterOrLeave;
            c.actionType = StateActionTypeEnum.ExecDelegate;
            c.dlg = dlg;
            stateActions.Add(c);
        }

        //класс хранит конкретное действие, которое делает объект при переходе в новое состояние
        private class StateAction
        {
            public int stateId;
            public ActionOnEnterOrLeaveEnum enterOrLeave;
            public StateActionTypeEnum actionType;
            public List<Control> targetControls;
            public string targetParameter;
            public object targetParameterValue;

            //public delegate void actionDelegate();
            public StateMachine.actionDelegate dlg;

            public StateAction()
            {
                
            }

            //собственно, выполняет действие
            public void doAction()
            {
                dlg();
            }
        }

        //очстить список действий
        public void clearActions()
        {
            stateActions.Clear();
        }
        
        //
        public enum StateActionTypeEnum
        {
            SetProperty = 1, //присвоение свойства
            Show = 2,  //показать контролы
            Hide = 3, //скрыть контролы
            ExecDelegate =4 //выполнить назначенный делегат
        }

        
        //делается ли действие на входе в состояние или на выходе из состояния
        public enum ActionOnEnterOrLeaveEnum 
        {
            Enter = 1, 
            Leave = 2
        }
    }



    

}
