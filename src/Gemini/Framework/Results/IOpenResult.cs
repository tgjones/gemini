using System;
using Caliburn.Micro;

namespace Gemini.Framework.Results
{
	public interface IOpenResult<TChild> : IResult
	{
		Action<TChild> OnConfigure { get; set; }
		Action<TChild> OnShutDown { get; set; }

		//void SetData<TData>(TData data);
	}
}