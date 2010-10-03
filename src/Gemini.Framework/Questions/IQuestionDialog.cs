using System.Collections.Generic;
using Caliburn.PresentationFramework.ApplicationModel;

namespace Gemini.Framework.Questions
{
	public interface IQuestionDialog : IExtendedPresenter
    {
        void Setup(string caption, IEnumerable<Question> questions);
    }
}