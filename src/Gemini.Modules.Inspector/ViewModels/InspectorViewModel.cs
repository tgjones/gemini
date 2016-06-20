using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Inspector.Properties;

namespace Gemini.Modules.Inspector.ViewModels
{
    [Export(typeof(IInspectorTool))]
    public class InspectorViewModel : Tool, IInspectorTool
    {
        public event EventHandler SelectedObjectChanged;

        public override PaneLocation PreferredLocation
        {
            get { return PaneLocation.Right; }
        }

        public override double PreferredWidth
        {
            get { return 300; }
        }

        private IInspectableObject _selectedObject;

        public IInspectableObject SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                NotifyOfPropertyChange(() => SelectedObject);
                RaiseSelectedObjectChanged();
            }
        }

        public InspectorViewModel()
        {
            DisplayName = Resources.InspectorDisplayName;
        }

        private void RaiseSelectedObjectChanged()
        {
            EventHandler handler = SelectedObjectChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void ResetAll()
        {
            if (SelectedObject == null)
                return;

            RecurseEditors(SelectedObject.Inspectors, delegate(Inspectors.IEditor editor) {
                if (editor != null && editor.CanReset)
                    editor.Reset();
            });
        }

        public void RecurseEditors(IEnumerable<Inspectors.IInspector> inspectors, Action<Inspectors.IEditor> action)
        {
            foreach (var inspector in inspectors)
            {
                var group = inspector as Inspectors.CollapsibleGroupViewModel;
                if (group != null)
                {
                    RecurseEditors(group.Children, action);
                }
                else
                {
                    action(inspector as Inspectors.IEditor);
                }
            }
        }
    }
}