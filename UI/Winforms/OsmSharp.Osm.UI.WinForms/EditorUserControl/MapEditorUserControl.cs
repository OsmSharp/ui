// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OsmSharp.Osm.Map.Layers.Custom;
using OsmSharp.Osm.Map.Layers.Tiles;
using OsmSharp.Tools.Math.Geo;
using OsmSharp.Osm.Renderer.Gdi.Targets.UserControlTarget;
using System.IO;
using OsmSharp.Tools.Xml.Gpx;
using OsmSharp.Tools.Xml.Sources;
using OsmSharp.Osm.Map.Layers;
using OsmSharp.Osm.UI.WinForms.EditorUserControl.Logic;
using System.Globalization;
using OsmSharp.Osm.Map.Elements;
using OsmSharp.Osm.Map;
using OsmSharp.Osm.UI.WinForms.Layers;

namespace OsmSharp.Osm.UI.WinForms.MapEditorUserControl
{
    public partial class MapEditorUserControl : UserControl
    {
        private MapEditorUserControlLogic _logic;

        public MapEditorUserControl()
        {
            InitializeComponent();

            _show_log = false;
            this.SelectionPixels = 10;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            // create the initial logic.
            _logic = new MapViewingLogic(this);
        }

        #region Properties

        public bool ShowToolBar { get; set; }

        private bool _show_log;

        public bool ShowLog 
        {
            get
            {
                return _show_log;
            }
            set
            {
                _show_log = value;

                this.splitContainer1.Panel2Collapsed = !_show_log;                
            }
        }

        /// <summary>
        /// Returns the current view.
        /// </summary>
        public OsmSharp.Osm.Renderer.View View
        {
            get
            {
                return this.mapTarget.View;
            }
        }

        /// <summary>
        /// Returns the target.
        /// </summary>
        public UserControlTarget Target
        {
            get
            {
                return this.mapTarget;
            }
        }

        /// <summary>
        /// Gets/Sets the map being displayed.
        /// </summary>
        public Map.Map Map
        {
            get
            {
                return this.Target.Map;
            }
            set
            {
                this.Target.Map = value;
            }
        }

        /// <summary>
        /// Gets/Sets the zoomfactor being displayed.
        /// </summary>
        public float ZoomFactor
        {
            get
            {
                return this.Target.ZoomFactor;
            }
            set
            {
                this.Target.ZoomFactor = value;
            }
        }

        /// <summary>
        /// Gets/Sets the center being displayed.
        /// </summary>
        public GeoCoordinate Center
        {
            get
            {
                return this.Target.Center;
            }
            set
            {
                this.Target.Center = value;
            }
        }

        /// <summary>
        /// Gets/Sets the layer being edited.
        /// </summary>
        public ILayer ActiveLayer 
        { 
            get; 
            set; 
        }

        #endregion

        #region Events -> Logics

        private void mapTarget_MapMouseDown(UserControlTargetEventArgs e)
        {
            _moved = false;
            _logic = _logic.OnMapMouseDown(e);
        }

        private Point? _move_location = null;
        private bool _moved = false;
        private void mapTarget_MapMouseMove(UserControlTargetEventArgs e)
        {
            _moved = true;
            if (_move_location != null && _move_location.HasValue)
            {
                if (_move_location.Value == e.Location)
                { // only trigger event when there was an actual move.
                    return;
                }
            }
            _move_location = e.Location;
            _logic = _logic.OnMapMouseMove(e);
        }

        private void mapTarget_MapMouseUp(UserControlTargetEventArgs e)
        {
            _logic = _logic.OnMapMouseUp(e);
        }

        private void mapTarget_MapMouseWheel(UserControlTargetEventArgs e)
        {
            _logic = _logic.OnMapMouseWheel(e);
        }

        private void mapTarget_MapMouseClick(UserControlTargetEventArgs e)
        {
            if (!_moved)
            {
                _moved = false;
                if (this.MapClick != null)
                {
                    this.MapClick(e);
                }

                GeoCoordinateBox box = this.mapTarget.View.CreateBoxAround(this.mapTarget, e.Position, this.SelectionPixels);

                MapQueryResult elements = this.Map.GetElementsAt(box, this.ZoomFactor);

                if (elements.Items.Count > 0)
                {
                    if (IElementClick != null)
                    {
                        IElementClick(elements.Items.ToList<IElement>());
                    }
                }
            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            this.Target.ZoomFactor = this.Target.ZoomFactor +
                1f;
            //this.Refresh();
        }

        private void tsbZoomOut_Click(object sender, EventArgs e)
        {
            this.Target.ZoomFactor = this.Target.ZoomFactor -
                1f;
            //this.Refresh();
        }

        public event MapClickDelegate MapClick;
        public delegate void MapClickDelegate(UserControlTargetEventArgs e);


        public event MapMoveDelegate MapMove;
        public delegate void MapMoveDelegate(UserControlTargetEventArgs e);
        private void RaiseMapMove(UserControlTargetEventArgs e)
        {
            if (MapMove != null)
            {
                this.MapMove(e);
            }
        }

        public event IElementClickDelegate IElementClick;     

        public int SelectionPixels { get; set; }

        public delegate void IElementClickDelegate(IList<IElement> elements);

        public event IElementClickDelegate IElementDoubleClick;

        private void mapTarget_MapMouseDoubleClick(UserControlTargetEventArgs e)
        {
            GeoCoordinateBox box = this.mapTarget.View.CreateBoxAround(this.mapTarget, e.Position, this.SelectionPixels);
            //GeoCoordinateBox box = new GeoCoordinateBox(
            //    new GeoCoordinate(e.Position.Latitude - 0.001, e.Position.Longitude - 0.001),
            //    new GeoCoordinate(e.Position.Latitude + 0.001, e.Position.Longitude + 0.001));

            MapQueryResult elements = this.Map.GetElementsAt(box,this.ZoomFactor);

            if (elements.Items.Count > 0)
            {
                if (IElementDoubleClick != null)
                {
                    IElementDoubleClick(elements.Items.ToList<IElement>());
                }
            }
        }

        public event IElementClickDelegate IElementMove;

        internal void MapMouseMoveWithoutMovingMap(UserControlTargetEventArgs e)
        {
            GeoCoordinateBox box = this.mapTarget.View.CreateBoxAround(this.mapTarget, e.Position, this.SelectionPixels);

            MapQueryResult elements = this.Map.GetElementsAt(box, this.ZoomFactor);

            // raise the mouse move event.
            this.RaiseMapMove(e);

            // raise the element move event.
            if (elements.Items.Count > 0)
            {
                if (IElementMove != null)
                {
                    IElementMove(elements.Items.ToList<IElement>());
                }
            }
        }


        public event IElementClickDelegate IElementsSelected;

        internal void MapSelectionEnd(GeoCoordinateBox box)
        {
            MapQueryResult elements = this.Map.GetElementsAt(box, this.ZoomFactor);

            if (elements.Items.Count > 0)
            {
                if (IElementsSelected != null)
                {
                    IElementsSelected(elements.Items.ToList<IElement>());
                }
            }

            this.mapTarget.ResetSelectiongBox();
            this.mapTarget.Invalidate();

        }

        #endregion

        #region ToolBar Events

        private void tsbOpenFile_Click(object sender, EventArgs e)
        {
            // show open file dialog.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // switch on file extension.
                FileInfo file = new FileInfo(openFileDialog1.FileName);
                switch (file.Extension.ToLower())
                {
                    //case ".gpx":
                    //    // build a new layer with the gpx file.
                    //    GpxDocument gpx_document = new GpxDocument(
                    //        new XmlFileSource(file));
                    //    GpxDataSource gpx_source = new GpxDataSource(
                    //        gpx_document);
                    //    DataSourceLayer gpx_layer = new DataSourceLayer(
                    //        gpx_source);

                    //    // add layer to the map.
                    //    this.Map.Layers.Add(gpx_layer);

                    //    break;
                    //case ".osm":
                    //    // build a new layer with the osm file.
                    //    OsmDocument osm_document = new OsmDocument(
                    //        new XmlFileSource(file));
                    //    OsmDataSource osm_source = new OsmDataSource(
                    //        osm_document);
                    //    DataSourceLayer osm_layer = new DataSourceLayer(
                    //        osm_source);

                    //    // add layer to the map.
                    //    this.Map.Layers.Add(osm_layer);

                    //    break;
                }
            }
        }


        private void tspAddPoi_Click(object sender, EventArgs e)
        {
            _logic = new MapEditLogicAddingNode(this);
        }


        private void tsbAddWay_Click(object sender, EventArgs e)
        {
            _logic = new MapEditLogicAddingWay(this);
        }

        #endregion

        private void tsbEditOnline_Click(object sender, EventArgs e)
        {
            if(!tsbEditOnline.Checked)
            {
                this.browserEditorUserControl1.Url = new Uri(
                    string.Format(@"http://www.openstreetmap.org/edit?lat={0}&lon={1}&zoom={2}",
                        this.mapTarget.Center.Latitude.ToString(CultureInfo.InvariantCulture),
                        this.mapTarget.Center.Longitude.ToString(CultureInfo.InvariantCulture),
                        ((int)this.mapTarget.ZoomFactor).ToString()));
                this.browserEditorUserControl1.Visible = true;
                this.mapTarget.Visible = false;
                tsbEditOnline.Checked = true;
            }
            else
            {
                this.browserEditorUserControl1.Visible = false;
                this.mapTarget.Visible = true;
                tsbEditOnline.Checked = false;
            }
        }
        
        internal void DrawSelectionBox(Point point, Point point_2)
        {
            this.mapTarget.DrawSelectionBox(point, point_2);
            this.mapTarget.Invalidate();
        }

        public bool SelectionMode 
        {
            get
            {
                return this.btnSelect.Checked;
            }
            set
            {
                this.btnSelect.Checked = false;
            }
        }

        private void btnLayers_Click(object sender, EventArgs e)
        {
            LayersToolsForm layers_form = new LayersToolsForm();
            layers_form.SetMap(this.mapTarget.Map);
            layers_form.ShowDialog();

            this.Refresh();
        }
    }
}
