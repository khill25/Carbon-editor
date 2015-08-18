using System;
using System.Collections.Generic;

namespace CarbonDatabase {
    
	public class Script : CarbonBaseObject {
	
		public override String AssemblyName { get {return "Script";} }
		public String ScriptingLanguage { get; set; }
		public Uri SourceFilePath {get;set;}
		public String SourceFileName {get;set;}
		public String Source { get; set; }

        // Are we going to even need this property?
        private List<ScriptItem> m_ScriptItems;
        public List<ScriptItem> ScriptItems {
            get { return m_ScriptItems ?? (m_ScriptItems = new List<ScriptItem>()); }
            private set { m_ScriptItems = value; }
        }

		private List<ScriptObject> m_ScriptObjects;
		public List<ScriptObject> ScriptObjects {
			get { return m_ScriptObjects ?? (m_ScriptObjects = new List<ScriptObject>()); }
			private set { m_ScriptObjects = value; }
		}
	
		public bool SaveScript() {
			return SaveScript(this.SourceFileName);
		}

		public bool SaveScript(String filename) {

			// TODO write the file to disk 
			// ...and make a record in the database???

			return false;

		}

    }
}
