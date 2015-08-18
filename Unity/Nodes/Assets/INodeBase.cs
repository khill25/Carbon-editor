using UnityEngine;
using System;
using System.Collections.Generic;
using CarbonDatabase;
using WyrmTale;

public interface INodeBase {

	object ConvertFromJson(JSON json);
	void SetupConnections();

}
