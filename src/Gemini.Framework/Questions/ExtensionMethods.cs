using System;
using System.Collections.Generic;
using Caliburn.Core;
using Microsoft.Practices.ServiceLocation;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Questions
{
	public static class ExtensionMethods
    {
        public static void Execute(this ISubordinate model, Action completed)
        {
            var dialog = ServiceLocator.Current.GetInstance<IQuestionDialog>();
            dialog.Setup("Warning", model.GetQuestions());
            dialog.WasShutdown += delegate { completed(); };
            ServiceLocator.Current.GetInstance<IWindowManager>().ShowDialog(dialog, null, Execute);
        }

        public static IEnumerable<Question> GetQuestions(this ISubordinate model)
        {
            var queue = new Queue<ISubordinate>();
            queue.Enqueue(model);

            while(queue.Count > 0)
            {
                var current = queue.Dequeue();
                var anotherCompoiste = current as ISubordinateComposite;

                if(anotherCompoiste != null)
                    anotherCompoiste.GetChildren().Apply(queue.Enqueue);
                else yield return (Question)current;
            }
        }
    }
}