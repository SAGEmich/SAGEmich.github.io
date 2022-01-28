using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace StateMachineNew
{
	public class NewStateMachine
	{
		private IStateMachine currentState;

		private Dictionary<Type, List<Transition>> transitions = new Dictionary<Type, List<Transition>>();
		private List<Transition> currentTransitions = new List<Transition>();
		private List<Transition> anyTransitions = new List<Transition>();
		
		private static List<Transition> emptyTransitions = new List<Transition>(0);

		public void Tick()
		{
			Transition transition = GetTransition();

			if(transition != null)
			{
				SetState(transition.To);
			}
			
			currentState?.Tick();
		}

		public void SetState(IStateMachine stateMachine)
		{
			//Blokada przed loopowaniem jednego stanu
			if(stateMachine == currentState)
				return;

			//Wychodzenie z obecnego stanu
			currentState?.OnExit();
			currentState = stateMachine;
			
			//Hej, dictionary daj mi liste przejść dla tego typu i przenieś do obecnego przejścia
			transitions.TryGetValue(currentState.GetType(), out currentTransitions);
			if(currentTransitions == null)
				currentTransitions = emptyTransitions;
			
			currentState.OnEnter();
		}

		public void AddTransition(IStateMachine from, IStateMachine to, Func<bool> predicate)
		{
			if(transitions.TryGetValue(from.GetType(), out List<Transition> _transitions) == false)
			{
				_transitions = new List<Transition>();
				transitions[from.GetType()] = _transitions;
			}
			
			_transitions.Add(new Transition(to, predicate));
		}

        public void AddAnyTransition(IStateMachine stateMachine, Func<bool> predicate)
        {
            anyTransitions.Add(new Transition(stateMachine, predicate));
        }
		
		private class Transition
		{
			public Func<bool> Condition { get; }
			public IStateMachine To { get; }

			public Transition(IStateMachine to, Func<bool> condition)
			{
				To = to;
				Condition = condition;
			}
		}

		private Transition GetTransition()
		{
			foreach(var transition in anyTransitions)
			{
				if(transition.Condition())
					return transition;
			}

			foreach(var transition in currentTransitions)
			{
				if(transition.Condition())
					return transition;
			}

			return null;
		}
	}
}

