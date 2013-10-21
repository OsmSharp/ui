using System.Text;
using OsmSharp.Collections.Tags;
using System;
using OsmSharp.Geo.Attributes;

namespace OsmSharp.UI.Map.Styles.MapCSS.v0_2.Domain
{
    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public abstract class Declaration
    {
        
    }

    /// <summary>
    /// Strongly typed MapCSS v0.2 Declaration class.
    /// </summary>
    public abstract class Declaration<TQualifier, TValue> : Declaration
    {
        /// <summary>
        /// The qualifier in this declaration.
        /// </summary>
        public TQualifier Qualifier { get; set; }

        /// <summary>
        /// The value of this declaration.
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// Holds the eval function.
        /// </summary>
        public string EvalFunction { get; set; }

        /// <summary>
        /// Evalues the value in this declaration or returns the regular value when there is no eval function.
        /// </summary>
        /// <param name="mapCSSObject"></param>
        /// <returns></returns>
        public virtual TValue Eval(MapCSSObject mapCSSObject)
        {
			if (!string.IsNullOrWhiteSpace (this.EvalFunction)) {
				throw new NotSupportedException (string.Format("Eval function not supported on this value declaration type: {0}.",
				                                this.GetType ().ToString ()));
			}
			return this.Value;
        }

        /// <summary>
        /// Returns a description of this declaration.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!string.IsNullOrWhiteSpace(this.EvalFunction))
            {
                return string.Format("{0}:{1}",
                                    this.ConvertToDashedFormat(this.Qualifier.ToString()),
                                    this.EvalFunction);
            }
            return string.Format("{0}:{1}",
                                 this.ConvertToDashedFormat(this.Qualifier.ToString()),
                                 this.ConvertToDashedFormat(this.Value.ToString()));
        }

        private string ConvertToDashedFormat(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var builder = new StringBuilder();
                builder.Append(char.ToLower(value[0]));
                for (int idx = 1; idx < value.Length; idx++)
                {
                    if (char.IsUpper(value[idx]))
                    {
                        builder.Append('-');
                        builder.Append(char.ToLower(value[idx]));
                    }
                    else
                    {
                        builder.Append(value[idx]);
                    }
                }
                return builder.ToString();
            }
            return string.Empty;
        }
    }
}