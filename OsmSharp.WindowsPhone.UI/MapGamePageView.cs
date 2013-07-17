using System;
using System.Net;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Windows.Navigation;
using PhoneClassLibrary4.Renderer;
using OsmSharp.Math.Geo;
using OsmSharp.UI.Map;
using OsmSharp.UI.Renderer;

namespace OsmSharp.WindowsPhone.UI
{
    public class MapGamePageView : PhoneApplicationPage
    {
        /// <summary>
        /// Holds the content manager.
        /// </summary>
        private ContentManager _contentManager;

        /// <summary>
        /// Holds the gametimer.
        /// </summary>
        private GameTimer _timer;

        /// <summary>
        /// Holds the sprite batch.
        /// </summary>
        private SpriteBatch _spriteBatch;

        /// <summary>
        /// Holds the renderer.
        /// </summary>
        private MapRenderer<GraphicsDevice> _renderer;

        /// <summary>
        /// Creates a new map game page view using the given content manager.
        /// </summary>
        /// <param name="contentManager"></param>
        public MapGamePageView(ContentManager contentManager)
        {
            // Get the content manager from the application
            _contentManager = contentManager;

            // Create a timer for this page
            _timer = new GameTimer();
            _timer.UpdateInterval = TimeSpan.FromTicks(333333);
            _timer.Update += OnUpdate;
            _timer.Draw += OnDraw;
        }

        #region Rendering

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set the sharing mode of the graphics device to turn on XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(true);

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(SharedGraphicsDeviceManager.Current.GraphicsDevice);

            // Initialize the graphics renderer.
            _renderer = new MapRenderer<GraphicsDevice>(new GraphicsDeviceRenderer2D());

            // TODO: use this.content to load your game content here

            // Start the timer
            _timer.Start();

            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Stop the timer
            _timer.Stop();

            // Set the sharing mode of the graphics device to turn off XNA rendering
            SharedGraphicsDeviceManager.Current.GraphicsDevice.SetSharingMode(false);

            base.OnNavigatedFrom(e);
        }

        /// <summary>
        /// Allows the page to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        private void OnUpdate(object sender, GameTimerEventArgs e)
        {
            // TODO: Add your update logic here
        }

        /// <summary>
        /// Allows the page to draw itself.
        /// </summary>
        private void OnDraw(object sender, GameTimerEventArgs e)
        {
            View2D view = this.CreateView();

            this.Map.ViewChanged((float)this.Map.Projection.ToZoomFactor(this.ZoomLevel),
                                 this.Center,
                                 view);

            _renderer.Render(SharedGraphicsDeviceManager.Current.GraphicsDevice, 
                this.Map, (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel), this.Center);
        }

        #endregion

        /// <summary>
        /// Creates the view.
        /// </summary>
        /// <returns>The view.</returns>
        public View2D CreateView()
        {
            double[] sceneCenter = this.Map.Projection.ToPixel(this.Center.Latitude, this.Center.Longitude);
            float sceneZoomFactor = (float)this.Map.Projection.ToZoomFactor(this.ZoomLevel);

            return View2D.CreateFrom(sceneCenter[0], sceneCenter[1],
                                     this.Width, this.Height, sceneZoomFactor,
                                     this.Map.Projection.DirectionX, this.Map.Projection.DirectionY);
        }

        /// <summary>
        /// Gets or sets the center.
        /// </summary>
        /// <value>The center.</value>
        public GeoCoordinate Center
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the map.
        /// </summary>
        /// <value>The map.</value>
        public Map Map
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the zoom factor.
        /// </summary>
        /// <value>The zoom factor.</value>
        public float ZoomLevel
        {
            get;
            set;
        }
    }
}
