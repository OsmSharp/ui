// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
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

using System.Diagnostics;
using MonoTouch.UIKit;

namespace OsmSharp.iOS.UI.Log
{
    /// <summary>
    /// A log trace listener that writes message to a given text view.
    /// </summary>
    public class TextViewTraceListener : TraceListener
    {
        /// <summary>
        /// Holds the textview to write to.
        /// </summary>
        private UITextView _textView;

        /// <summary>
        /// Creates a new text view trace listener.
        /// </summary>
        /// <param name="textView"></param>
        public TextViewTraceListener(UITextView textView)
        {
            _textView = textView;
        }

        /// <summary>
        /// Writes the given message to the textview.
        /// </summary>
        /// <param name="message"></param>
        public override void Write(string message)
        {
            _textView.InvokeOnMainThread(() =>
            {
                _textView.Text = _textView.Text + message;
                _textView.ScrollRangeToVisible(
                    new MonoTouch.Foundation.NSRange(_textView.Text.Length, 0));
            });
        }

        /// <summary>
        /// Writes the given message to the textview.
        /// </summary>
        /// <param name="message"></param>
        public override void WriteLine(string message)
        {
            _textView.InvokeOnMainThread(() =>
            {
                _textView.Text = _textView.Text + message + System.Environment.NewLine;
                _textView.ScrollRangeToVisible(
                    new MonoTouch.Foundation.NSRange(_textView.Text.Length, 0));
            });
        }
    }
}