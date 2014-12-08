﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Caliburn.Micro;
using Gemini.Demo.Modules.Home.Views;
using Gemini.Framework;
using Gemini.Modules.CodeCompiler;
using Roslyn.Compilers;
using Roslyn.Compilers.CSharp;

namespace Gemini.Demo.Modules.Home.ViewModels
{
    [Export(typeof(HelixViewModel))]
    public class HelixViewModel : Document
    {
        private readonly ICodeCompiler _codeCompiler;
        private readonly List<IDemoScript> _scripts;

        private IHelixView _helixView;

        private Point3D _cameraPosition;
        [DisplayName("Camera Position"), Description("Position of the camera in 3D space"), Category("Camera")]
        public Point3D CameraPosition
        {
            get { return _cameraPosition; }
            set
            {
                _cameraPosition = value;
                NotifyOfPropertyChange(() => CameraPosition);
            }
        }

        private double _cameraFieldOfView;
        [DisplayName("Field of View"), Range(1.0, 180.0), Category("Camera")]
        public double CameraFieldOfView
        {
            get { return _cameraFieldOfView; }
            set
            {
                _cameraFieldOfView = value;
                NotifyOfPropertyChange(() => CameraFieldOfView);
            }
        }

        private Point3D _lightPosition;
        [DisplayName("Light Position")]
        public Point3D LightPosition
        {
            get { return _lightPosition; }
            set
            {
                _lightPosition = value;
                NotifyOfPropertyChange(() => LightPosition);
            }
        }

        private double _rotationAngle;
        [DisplayName("Rotation Angle"), ReadOnly(true)]
        public double RotationAngle
        {
            get { return _rotationAngle; }
            set
            {
                _rotationAngle = value;
                NotifyOfPropertyChange(() => RotationAngle);
            }
        }

        private double? _nullableDoubleValue;
        [DisplayName(@"Nullable Double Value")]
        public double? NullableDoubleValue
        {
            get { return _nullableDoubleValue; }
            set
            {
                if (value == _nullableDoubleValue) return;
                _nullableDoubleValue = value;
                NotifyOfPropertyChange(() => NullableDoubleValue);
            }
        }

        [ImportingConstructor]
        public HelixViewModel(ICodeCompiler codeCompiler)
        {
            DisplayName = "Helix";

            _codeCompiler = codeCompiler;
            _scripts = new List<IDemoScript>();

            CameraPosition = new Point3D(6, 5, 4);
            CameraFieldOfView = 45;
            LightPosition = new Point3D(0, 5, 0);
            RotationAngle = 0;
        }

        protected override void OnViewLoaded(object view)
        {
            _helixView = (IHelixView) view;

            _helixView.TextEditor.Text = @"public class MyClass : Gemini.Demo.Modules.Home.ViewModels.IDemoScript
{
    public void Execute(Gemini.Demo.Modules.Home.ViewModels.HelixViewModel viewModel)
    {
        viewModel.RotationAngle += 0.1;
    }
}
";

            _helixView.TextEditor.TextChanged += (sender, e) => CompileScripts();
            CompositionTarget.Rendering += OnRendering;
            CompileScripts();

            base.OnViewLoaded(view);
        }

        private void CompileScripts()
        {
            lock (_scripts)
            {
                _scripts.Clear();

                var newAssembly = _codeCompiler.Compile(
                    new[] { SyntaxTree.ParseText(_helixView.TextEditor.Text) },
                    new[]
                        {
                            MetadataReference.CreateAssemblyReference("mscorlib"),
                            MetadataReference.CreateAssemblyReference("System"),
                            MetadataReference.CreateAssemblyReference("System.ObjectModel"),
                            MetadataReference.CreateAssemblyReference("System.Runtime"),
                            MetadataReference.CreateAssemblyReference("PresentationCore"),
                            new MetadataFileReference(typeof(IResult).Assembly.Location),
                            new MetadataFileReference(typeof(AppBootstrapper).Assembly.Location),
                            new MetadataFileReference(GetType().Assembly.Location)
                        },
                    "GeminiDemoScript");

                _scripts.AddRange(newAssembly.GetTypes()
                    .Where(x => typeof(IDemoScript).IsAssignableFrom(x))
                    .Select(x => (IDemoScript) Activator.CreateInstance(x)));
            }
        }

        private void OnRendering(object sender, EventArgs e)
        {
            lock (_scripts)
                _scripts.ForEach(x => x.Execute(this));
        }

        protected override void OnDeactivate(bool close)
        {
            if (close)
                CompositionTarget.Rendering -= OnRendering;
            base.OnDeactivate(close);
        }
    }
}