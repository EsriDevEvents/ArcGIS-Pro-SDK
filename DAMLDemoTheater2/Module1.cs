using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;

namespace DAMLDemoTheater
{
  internal class Module1 : Module
  {
    private static Module1 _this = null;

    /// <summary>
    /// Retrieve the singleton instance to this module here
    /// </summary>
    public static Module1 Current => _this ??= (Module1)FrameworkApplication.FindModule("DAMLDemoTheater_Module");

    #region Overrides
    protected override bool Initialize()
    {
      // listen to the active map view changed event to update the state of the condition
      ArcGIS.Desktop.Mapping.Events.ActiveMapViewChangedEvent.Subscribe(OnActiveMapViewChanged);
      return base.Initialize();
    }

    private void OnActiveMapViewChanged(ArcGIS.Desktop.Mapping.Events.ActiveMapViewChangedEventArgs args)
    {
      // args.IncomingView contains the new active map view (can be null)
      // args.OutgoingView contains the previous active map view (can be null)
      bool stateActive = false;
      if (args.IncomingView != null)
      {
        // A map view became active
        stateActive = args.IncomingView.Map.GetLayersAsFlattenedList().OfType<NetworkDatasetLayer>().Any();
      }
      else
      {
        // No map view is active
      }
      // Activate or deactivate the state based on whether the active map view contains a NetworkDatasetLayer
      if (stateActive)
        FrameworkApplication.State.Activate("NetworkDatasetLayerinMap_State");
      else
        FrameworkApplication.State.Deactivate("NetworkDatasetLayerinMap_State");
    }

    /// <summary>
    /// Called by Framework when ArcGIS Pro is closing
    /// </summary>
    /// <returns>False to prevent Pro from closing, otherwise True</returns>
    protected override bool CanUnload()
    {
      //TODO - add your business logic
      //return false to ~cancel~ Application close
      return true;
    }

    #endregion Overrides

  }
}
