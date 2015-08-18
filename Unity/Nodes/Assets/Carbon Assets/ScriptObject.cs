using System;
using System.Collections.Generic;

namespace CarbonDatabase {
    /// <summary>
    /// ScriptObject represents any thing in the world that is not a actor. This is generally used for locations or settings
    /// It can also be used to create sub categories, folders. just about anything.
    /// It's very vanilla and plain for the reason that it can be any thing that we might need.
    /// Also saves me from writing a ton of static tables like location, scene, tag, etc
    /// </summary>
	public class ScriptObject : CarbonBaseObject {

		public override String AssemblyName { get {return "ScriptObject";} }
        public String Category { get; set; }
        public Script ParentScript { get; set; }
		protected String refernecedObjectType { get; set; }
		// Based on referencedObjectType
		public Object ReferencedObject { get; set; }
    }
}
