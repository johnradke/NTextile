using System;
using System.Collections.Generic;
using System.Linq;

namespace Textile
{
    public class FormatterStateManager
    {
        private readonly GenericFormatter _formatter;
        private Stack<FormatterState> _states = new Stack<FormatterState>();

        public FormatterStateManager(GenericFormatter formatter)
        {
            _formatter = formatter;
        }

        public void PushState(FormatterState state)
        {
            state.Formatter = _formatter;
            _states.Push(state);
            state.Enter();
        }

        public void PopState()
        {
            var state = _states.Pop();
            state.Exit();
            state.Formatter = null;
        }

		public FormatterState CurrentState => _states.Any() ? _states.Peek() : null;

        public void ChangeState<T>() where T:FormatterState, new()
        {
            ChangeState(new T());
        }

        public void ChangeState(FormatterState formatterState)
        {
            if (CurrentState != null && CurrentState.GetType() == formatterState.GetType())
            {
                if (!CurrentState.ShouldNestState(formatterState))
                {
                    return;
                }
            }

            PushState(formatterState);
        }

        public void PopAll()
        {
            while (_states.Any())
            {
                _states.Pop();
            }
        }

        public bool ShouldExit(string input, string inputLookAhead)
            => CurrentState?.ShouldExit(input, inputLookAhead) ?? false;
    }
}
