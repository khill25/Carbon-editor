using System;
using System.Collections.Generic;

namespace CarbonDatabase {
    /// <summary>
    ///  An entity can be anything in the game space. Basically the subject of the action.
    ///  It isn't only characters. It can be a lamp post or a computer monitor as well.
    /// </summary>
	public class Entity : CarbonBaseObject {

		public override String AssemblyName { get {return "Entity";} }
        public String Notes { get; set; }
        public String ExtendedInfo { get; set; }
        public int Type { get; set; }
        public Script AssociatedScript { get; set; }

		public List<CarbonAttribute> attributes = new List<CarbonAttribute> ();
        /// <summary>
        /// Trys to find the specified attribute by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns the found Attribute. Null if not found.</returns>
        public CarbonAttribute FindAttributeForKey(String key) {
            foreach (CarbonAttribute attribute in attributes) {
                if (attribute.Key.Equals(key)) {
                    return attribute;
                }
            }

            return null;
        }

        /// <summary>
        /// Add a new attribute if one doesn't already exist.
        /// </summary>
        /// <param name="name">Name of the attribute being added</param>
        /// <param name="value">Value of the atribute being added(String)</param>
        /// <param name="gamescript">Optional paramater specifing runnable game script</param>
        /// <returns>True if the item was added, false if the item exists</returns>
        public bool AddAttribute(String name, String value, String gamescript = null) {
            return false;
        }

        public void AddAttribute(CarbonAttribute att) {
//            var link = new AttributeActorJoin();
//            link.Actor = this;
//            link.Attribute = att;
//            Attributes.Add(link);
        }

        public bool RemoveAttribute(String name) {
            return false;
        }

    }
}
