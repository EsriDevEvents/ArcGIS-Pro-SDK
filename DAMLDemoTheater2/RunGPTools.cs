/*

   Copyright 2024 Esri

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       https://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.

   See the License for the specific language governing permissions and
   limitations under the License.

*/
using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Core.Geoprocessing;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.KnowledgeGraph;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAMLDemoTheater
{
  internal class RunGPTools : Button
  {
    protected override void OnClick()
    {
      // get the first selected ND layer
      // for use in the GeoProcessing tool
      if (MapView.Active?.Map == null) return;
      var Map = MapView.Active.Map;
      var layers = Map.GetLayersAsFlattenedList().OfType<NetworkDatasetLayer>();
      if (!layers.Any())
      {
        MessageBox.Show("Couldn't find a network dataset layer.");
        return;
      }

      try
      {
        _ = RunGPToolAsync(layers.First());
      }
      catch (Exception ex)
      {
        MessageBox.Show($@"Error: {ex.Message}");
      }
    }

    private async Task RunGPToolAsync(NetworkDatasetLayer layer)
    {
      //// create and initialize the progress dialog
      //// Note: Progress dialogs are not displayed when debugging in Visual Studio
      var valueArray = await QueuedTask.Run<IReadOnlyList<string>>(() =>
      {
        var valueArray = Geoprocessing.MakeValueArray(layer.Name, false);
        return valueArray;
      });
      var gpResult = await Geoprocessing.ExecuteToolAsync("na.BuildNetwork", valueArray); //, null, progsrc.Progressor);
      if (gpResult.IsFailed)
      {
        // display error messages if the tool fails, otherwise shows the default messages
        if (gpResult.Messages.Count() != 0)
        {
          Geoprocessing.ShowMessageBox(gpResult.Messages, "Build Network results", // progsrc.Message,
                          gpResult.IsFailed ?
                          GPMessageBoxStyle.Error : GPMessageBoxStyle.Default);
        }
        else
        {
          MessageBox.Show($@"Build Network results failed with error code, check parameters.");
        }
      }
    }
  }
}
