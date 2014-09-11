using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Java.Nio;
using Java.Lang;
using Android.Graphics.Drawables;
using Android.Content.Res;

namespace OsmSharp.Android.UI.Images.NinePatchHelpers
{
    class NinePatchChunk
    {
            /**
	     * The 9 patch segment is not a solid color.
	     */
	    public static int NO_COLOR = 0x00000001;

	    /**
	     * The 9 patch segment is completely transparent.
	     */
	    public static int TRANSPARENT_COLOR = 0x00000000;

	    /**
	     * Default density for image loading from some InputStream
	     */
	    public static int DEFAULT_DENSITY = 160;

        /// <summary>
        /// By default it's true
        /// </summary>
	    public bool wasSerialized = true;

        /// <summary>
        /// Horizontal stretchable areas list.
        /// </summary>
	    public List<Div> xDivs;

        /// <summary>
        /// Vertical stretchable areas list.
        /// </summary>
	    public List<Div> yDivs;

        /// <summary>
        /// Content padding
        /// </summary>
	    public Rect padding = new Rect();

        /// <summary>
        /// Holds the colors.
        /// </summary>
        private int[] colors;

        /// <summary>
        /// Creates a ninepatch drawable.
        /// </summary>
        /// <param name="resources"></param>
        /// <param name="bitmap"></param>
        /// <param name="srcName"></param>
        /// <returns></returns>
        public static NinePatchDrawable createNinePatchDrawable(Resources resources, Bitmap bitmap, string srcName)
        {
            NinePatchChunk chunk = CreateChunkFromRawBitmap(bitmap);
            return new NinePatchDrawable(resources, ModifyBitmap(resources, bitmap, chunk), chunk.toBytes(), chunk.padding, srcName);
        }

        protected static Bitmap ModifyBitmap(Resources resources, Bitmap bitmap, NinePatchChunk chunk)
        {
            Bitmap content = Bitmap.CreateBitmap(bitmap, 1, 1, bitmap.Width - 2, bitmap.Height - 2);
            int targetDensity = (int)resources.DisplayMetrics.DensityDpi;
            float densityChange = (float)targetDensity / bitmap.Density;
            if (densityChange != 1f)
            {
                int dstWidth =(int)System.Math.Round(content.Width * densityChange);
                int dstHeight = (int)System.Math.Round(content.Height * densityChange);
                content = Bitmap.CreateScaledBitmap(content, dstWidth, dstHeight, true);
                content.Density = targetDensity;
                chunk.padding = new Rect((int)System.Math.Round(chunk.padding.Left * densityChange),
                        (int)System.Math.Round(chunk.padding.Top * densityChange),
                        (int)System.Math.Round(chunk.padding.Right * densityChange),
                        (int)System.Math.Round(chunk.padding.Bottom * densityChange));

                recalculateDivs(densityChange, chunk.xDivs);
                recalculateDivs(densityChange, chunk.yDivs);
            }
            bitmap = content;
            return content;
        }

        private static void recalculateDivs(float densityChange, List<Div> divs) {
			foreach (Div div in divs) {
                div.start = (int)System.Math.Round(div.start * densityChange);
                div.stop = (int)System.Math.Round(div.stop * densityChange);
			}
		}

        /**
         * Serializes current chunk instance to byte array. This array will pass thia check: NinePatch.isNinePatchChunk(byte[] chunk)
         *
         * @return The 9-patch data chunk describing how the underlying bitmap is split apart and drawn.
         */
        public byte[] toBytes() {
		    int capacity = 4 + (7 * 4) + xDivs.Count * 2 * 4 + yDivs.Count * 2 * 4 + colors.Length * 4;
		    var byteBuffer = ByteBuffer.Allocate(capacity).Order(ByteOrder.NativeOrder());
		    byteBuffer.Put(Integer.ValueOf(1).ByteValue());
		    byteBuffer.Put(Integer.ValueOf(xDivs.Count * 2).ByteValue());
		    byteBuffer.Put(Integer.ValueOf(yDivs.Count * 2).ByteValue());
		    byteBuffer.Put(Integer.ValueOf(colors.Length).ByteValue());
		    //Skip
		    byteBuffer.PutInt(0);
		    byteBuffer.PutInt(0);

		    if (padding == null)
			    padding = new Rect();
		    byteBuffer.PutInt(padding.Left);
		    byteBuffer.PutInt(padding.Right);
		    byteBuffer.PutInt(padding.Top);
		    byteBuffer.PutInt(padding.Bottom);

		    //Skip
		    byteBuffer.PutInt(0);

		    foreach (Div div in xDivs) {
			    byteBuffer.PutInt(div.start);
			    byteBuffer.PutInt(div.stop);
		    }
		    foreach (Div div in yDivs) {
			    byteBuffer.PutInt(div.start);
			    byteBuffer.PutInt(div.stop);
		    }
            foreach (int color in colors)
            {
                byteBuffer.PutInt(color);
            }

            var bytes = new byte[capacity];
            byteBuffer.Rewind();
            byteBuffer.Get(bytes);
            return bytes;
	    }

        /// <summary>
        /// Creates a ninepatch chunkc from a raw image.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static NinePatchChunk CreateChunkFromRawBitmap(Bitmap bitmap)
        {
		    var chunck = new NinePatchChunk();
		    SetupStretchableRegions(bitmap, chunck);
		    SetupPadding(bitmap, chunck);
		    SetupColors(bitmap, chunck);
            return chunck;
	    }

        /// <summary>
        /// Setup colors.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="chunck"></param>
        private static void SetupColors(Bitmap bitmap, NinePatchChunk chunck)
        {            		
		    int bitmapWidth = bitmap.Width - 2;
		    int bitmapHeight = bitmap.Height - 2;
		    var xRegions = GetRegions(chunck.xDivs, bitmapWidth);
		    var yRegions = GetRegions(chunck.yDivs, bitmapHeight);
		    chunck.colors = new int[xRegions.Count * yRegions.Count];

		    int colorIndex = 0;
		    foreach (Div yDiv in yRegions) {
			    foreach (Div xDiv in xRegions) {
				    int startX = xDiv.start + 1;
				    int startY = yDiv.start + 1;
				    if (HasSameColor(bitmap, startX, xDiv.stop + 1, startY, yDiv.stop + 1)) {
					    int pixel = bitmap.GetPixel(startX, startY);
					    if (isTransparent(pixel))
						    pixel = TRANSPARENT_COLOR;
					    chunck.colors[colorIndex] = pixel;
				    } else {
                        chunck.colors[colorIndex] = NO_COLOR;
				    }
				    colorIndex++;
			    }
		    }
        }

        private static List<Div> GetRegions(List<Div> divs, int max) {
		    var divsOut = new List<Div>();
            if (divs == null || divs.Count == 0) return divsOut;
            for (int i = 0; i < divs.Count; i++)
            {
			    Div div = divs[i];
			    if (i == 0 && div.start != 0) {
				    divsOut.Add(new Div(0, div.start - 1));
			    }
			    if (i > 0) {
				    divsOut.Add(new Div(divs[i - 1].stop, div.start - 1));
			    }
			    divsOut.Add(new Div(div.start, div.stop - 1));
                if (i == divs.Count - 1 && div.stop < max)
                {
				    divsOut.Add(new Div(div.stop, max - 1));
			    }
		    }
		    return divsOut;
	    }


        private static bool HasSameColor(Bitmap bitmap, int startX, int stopX, int startY, int stopY)
        {
            int color = bitmap.GetPixel(startX, startY);
            for (int x = startX; x <= stopX; x++)
            {
                for (int y = startY; y <= stopY; y++)
                {
                    if (color != bitmap.GetPixel(x, y))
                        return false;
                }
            }
            return true;
        }

        private static void SetupPadding(Bitmap bitmap, NinePatchChunk chunck)
        {
		    int maxXPixels = bitmap.Width - 2;
		    int maxYPixels = bitmap.Height - 2;
            var xPaddings = GetXDivs(bitmap, bitmap.Height - 1);
		    if (xPaddings.Count > 1)
			    throw new System.Exception("Raw padding is wrong. Should be only one horizontal padding region");
		    var yPaddings = GetYDivs(bitmap, bitmap.Width - 1);
		    if (yPaddings.Count > 1)
                throw new System.Exception("Column padding is wrong. Should be only one vertical padding region");
		    if (xPaddings.Count == 0) xPaddings.Add(chunck.xDivs[0]);
		    if (yPaddings.Count == 0) yPaddings.Add(chunck.yDivs[0]);
		    chunck.padding = new Rect();
		    chunck.padding.Left = xPaddings[0].start;
		    chunck.padding.Right = maxXPixels - xPaddings[0].stop;
		    chunck.padding.Top = yPaddings[0].start;
		    chunck.padding.Bottom = maxYPixels - yPaddings[0].stop;
        }

        private static void SetupStretchableRegions(Bitmap bitmap, NinePatchChunk chunck)
        {
	        chunck.xDivs = GetXDivs(bitmap, 0);
		    if (chunck.xDivs.Count == 0)
                throw new System.Exception("must be at least one horizontal stretchable region");
		    chunck.yDivs = GetYDivs(bitmap, 0);
		    if (chunck.yDivs.Count == 0)
                throw new System.Exception("must be at least one vertical stretchable region");
        }

        private static List<Div> GetYDivs(Bitmap bitmap, int column) {
		    var yDivs = new List<Div>();
		    Div tmpDiv = null;
		    for (int i = 1; i < bitmap.Height; i++) {
			    tmpDiv = ProcessChunk(bitmap.GetPixel(column, i), tmpDiv, i - 1, yDivs);
		    }
		    return yDivs;
	    }

	    private static List<Div> GetXDivs(Bitmap bitmap, int raw) {
		   var xDivs = new List<Div>();
		    Div tmpDiv = null;
		    for (int i = 1; i < bitmap.Width; i++) {
			    tmpDiv = ProcessChunk(bitmap.GetPixel(i, raw), tmpDiv, i - 1, xDivs);
		    }
		    return xDivs;
	    }

	    private static Div ProcessChunk(int pixel, Div tmpDiv, int position, List<Div> divs) {
		    if (isBlack(pixel)) {
			    if (tmpDiv == null) {
				    tmpDiv = new Div();
				    tmpDiv.start = position;
			    }
		    }
		    if (isTransparent(pixel)) {
			    if (tmpDiv != null) {
				    tmpDiv.stop = position;
				    divs.Add(tmpDiv);
				    tmpDiv = null;
			    }
		    }
		    return tmpDiv;
	    }

        private static bool isTransparent(int color) {
		    return Color.GetAlphaComponent(color) == Color.Transparent;
	    }

	    private static bool isBlack(int pixel) {
		    return pixel == Color.Black;
	    }
    }
}