using System;
using System.ComponentModel.Composition;

namespace Gemini.Framework
{
    /// <summary>
    /// This is a valid <see cref="ExportAttribute"/>.
    /// Used to remove ambiguity between multiple exports of an object when the the Gemini framework must create an instance of it.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class GeminiExportAttribute : ExportAttribute
    {
        public GeminiExportAttribute() :
            base()
        {

        }

        public GeminiExportAttribute(Type contractType)
            : base(contractType)
        {

        }

        public GeminiExportAttribute(string contractName)
            : base(contractName)
        {

        }

        public GeminiExportAttribute(string contractName, Type contractType)
            : base(contractName, contractType)
        {

        }
    }
}