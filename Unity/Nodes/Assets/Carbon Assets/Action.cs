using System;
using System.Collections.Generic;

namespace CarbonDatabase {
    public class ActionItem : CarbonBaseObject {

		public override String AssemblyName { get {return "ActionItem";} }

        /// <summary>
        /// This is what the action is attached to
        /// </summary>
        public Entity AttachedTo { get; set; }
  
        public enum ActionType {
            None,
            Dialog,
            Video,
            Text,
            Audio,
            CameraAction
        }

		public ActionType type { get; set; }
        public Script Script { get; set; }
    }
}
