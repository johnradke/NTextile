using System;
using System.Collections.Generic;
using System.Linq;

namespace Textile
{
    public class FormatterStateManager
    {
        private readonly Type _defaultFallbackType;
        private readonly GenericFormatter _formatter;
        private readonly Stack<FormatterState> _states = new Stack<FormatterState>();

        public FormatterStateManager(GenericFormatter formatter, Type defaultFallbackType)
        {
            _formatter = formatter;
            _defaultFallbackType = defaultFallbackType;
        }

        public void PushState(FormatterState state)
        {
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

        public void ChangeState(Type t)
        {
            ChangeState((FormatterState)Activator.CreateInstance(t));
        }

        public void ChangeState(FormatterState state)
        {
            if (CurrentState != null && CurrentState.GetType() == state.GetType())
            {
                if (!CurrentState.ShouldNestState(state))
                {
                    return;
                }
            }

            state.Formatter = _formatter;
            _states.Push(state);
            state.Enter();
        }

        public void PopAll()
        {
            while (_states.Any())
            {
                PopState();
            }
        }

        public void Fallback()
        {
            if (CurrentState != null)
            {
                if (CurrentState.FallbackFormattingState != null)
                {
                    ChangeState(CurrentState.FallbackFormattingState);
                }
            }
            else
            {
                ChangeState(_defaultFallbackType);
            }
        }

        public bool ShouldExit(string input, string inputLookAhead)
            => CurrentState != null && CurrentState.ShouldExit(input, inputLookAhead);
    }
}
