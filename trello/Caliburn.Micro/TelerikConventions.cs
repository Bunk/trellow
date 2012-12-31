using System.Linq;
using System.Reflection;
using System.Windows.Data;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Primitives;

namespace Caliburn.Micro
{
    public static class TelerikConventions
    {
        public static void Install()
        {
            ConventionManager.AddElementConvention<RadBusyIndicator>(RadBusyIndicator.IsRunningProperty,
                                                                     "IsRunning",
                                                                     "Loaded");
            ConventionManager.AddElementConvention<RadDataBoundListBox>(DataControlBase.ItemsSourceProperty,
                                                                        "SelectedItem",
                                                                        "SelectionChanged")
                .ApplyBinding = (viewModelType, path, property, element, convention) =>
                {
                    if (!ConventionManager.SetBindingWithoutBindingOrValueOverwrite(viewModelType,
                                                                                    path,
                                                                                    property, element, convention,
                                                                                    DataControlBase.ItemsSourceProperty))
                        return false;

                    if (ConventionManager.HasBinding(element, RadDataBoundListBox.SelectedItemProperty))
                        return true;

                    var index = path.LastIndexOf('.');
                    index = index == -1 ? 0 : index + 1;
                       
                    var baseName = path.Substring(index);
                    foreach (var selectionPath in
                        from potentialName in ConventionManager.DerivePotentialSelectionNames(baseName)
                        where
                            viewModelType.GetProperty(potentialName,
                                                      BindingFlags.IgnoreCase | BindingFlags.Public |
                                                      BindingFlags.Instance) != null
                        select path.Replace(baseName, potentialName))
                    {
                        var binding = new Binding(selectionPath) {Mode = BindingMode.TwoWay};
                        BindingOperations.SetBinding(element, RadDataBoundListBox.SelectedItemProperty, binding);
                    }

                    return true;
                };
        }
    }
}
