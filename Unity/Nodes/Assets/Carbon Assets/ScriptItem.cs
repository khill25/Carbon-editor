using System;
using System.Collections.Generic;

namespace CarbonDatabase {
    /// <summary>
    /// This class has one action that defines that this node does,
    /// It can include any number of actors in the scene
    /// The text property is for any text that needs to be filled out by the item
    /// </summary>
	public class ScriptItem : CarbonBaseObject {

		public override String AssemblyName { get {return "ScriptItem";} }
        public int itemOrder { get; set; }

		// This list can be actions, other scripts, more script items
		public List<CarbonBaseObject> associatedObjects { get; set; }

        public Script script { get; set; }

		public List<Entity> actors = new List<Entity>();
        public void AddActor(Entity a) {
            //var link = new ActorScriptJoin();
            //link.Actor = a;
            //link.ScriptItem = this;
            //ActorLinks.Add(link);
        }

		public bool RemoveActor(Entity a) {
			return false;
		}

    }
}
