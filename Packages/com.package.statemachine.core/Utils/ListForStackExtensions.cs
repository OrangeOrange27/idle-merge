using System;
using System.Collections.Generic;
using System.Linq;

namespace Package.StateMachine.Utils
{
	public static class ListForStackExtensions
	{
		public static void Push<T>(this List<T> stack, T item)
		{
			stack.Add(item);
		}

		public static void PushRange<T>(this List<T> stack, IEnumerable<T> items)
		{
			stack.AddRange(items.Reverse());
		}

		public static T Pop<T>(this List<T> stack)
		{
			if (stack.Count == 0) throw new InvalidOperationException("The stack is empty");

			var lastElementInList = stack.Count - 1;
			var itemAtTopOfStack = stack[lastElementInList];
			stack.RemoveAt(lastElementInList);
			return itemAtTopOfStack;
		}

		public static T PopBottom<T>(this IList<T> stack)
		{
			if (stack.Count == 0) throw new InvalidOperationException("The stack is empty");

			var item = stack[0];
			stack.RemoveAt(0);
			return item;
		}

		public static T Peek<T>(this List<T> stack)
		{
			if (stack.Count == 0) throw new InvalidOperationException("The stack is empty");

			return stack[stack.Count - 1];
		}
	}
}
