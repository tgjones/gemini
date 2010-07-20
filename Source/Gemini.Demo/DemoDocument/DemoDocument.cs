using System.ComponentModel.Composition;
using System.Windows.Media.Media3D;
using Gemini.Contracts;
using Gemini.Contracts.Gui.Layout;

namespace Gemini.Demo.DemoDocument
{
	[Export(ContractNames.ExtensionPoints.Workbench.Documents, typeof(IDocument))] // so the Workbench can save/restore
	[Export(DemoContractNames.CompositionPoints.PinBall.PinBallTable, typeof(DemoDocument))] // so the new View Menu item can refer to it
	[Document(Name = DemoDocument.DOCUMENT_NAME)]
	public class DemoDocument : AbstractDocument
	{
		public const string DOCUMENT_NAME = "DemoDocument";

		private Camera _camera;

		public DemoDocument()
		{
			Name = DOCUMENT_NAME;
			Title = "Demo Document";

			_camera = new PerspectiveCamera
			{
				Position = new Point3D(6, 5, 4),
				LookDirection = new Vector3D(-6, -5, -4)
			};
		}

		public Camera Camera
		{
			get { return _camera; }
		}
	}
}