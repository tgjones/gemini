using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Gemini.Demo.Xna.Primitives;
using Gemini.Framework;
using Gemini.Framework.Services;
using Microsoft.Xna.Framework;

namespace Gemini.Demo.Xna.Modules.PrimitiveList.ViewModels
{
    [Export(typeof(PrimitiveListViewModel))]
    public class PrimitiveListViewModel : Tool
    {
        private readonly List<PrimitiveWithColor> _primitives;

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        public IList<PrimitiveWithColor> Primitives
        {
            get { return _primitives; }
        }

        public PrimitiveListViewModel()
        {
            DisplayName = "Primitive List";

            _primitives = new List<PrimitiveWithColor>(
                new[] { Color.Blue, Color.Red, Color.Yellow, Color.Green, Color.Gold, Color.Fuchsia, Color.Black, Color.SlateBlue }
                    .Select(x => new PrimitiveWithColor
                    {
                        Primitive = new CubePrimitive(),
                        Color = x
                    }));
        }
    }

    public class PrimitiveWithColor
    {
        public GeometricPrimitive Primitive { get; set; }
        public Color Color { get; set; }
    }
}