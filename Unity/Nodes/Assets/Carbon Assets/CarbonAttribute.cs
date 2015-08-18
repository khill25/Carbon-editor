using System;
using System.Collections.Generic;

namespace CarbonDatabase {
    /// <summary>
    ///  An actor can be anything in the game space. Basically the subject of the action.
    ///  It isn't only characters. It can be a lamp post or a computer monitor as well.
    /// </summary>
    public class CarbonAttribute : CarbonBaseObject, IEquatable<CarbonAttribute> {

		public override String AssemblyName { get {return "CarbonAttribute";} }
		public String Key { get; set; }
        public Object Value { get; set; }
        public Script Script { get; set; }

		public CarbonAttribute(String key, Object value) : base() {
			this.Key = key;
			this.Value = value;
		}

		public bool Equals(CarbonAttribute other) {
			return other.Key.Equals(this.Key) && other.Value.Equals(this.Value);
		}
    }
}

