# OxideNoLimit

This project was absorbed by officially method available to disable the plugin sandbox.  
Please **remove** it now.  

### Disabling Sandbox (new way)
  1. Go to `YourServerRootDir/HarmonyMods` directory and delete `OxideNoLimit.dll` (server must be offline)  
  2. Go to `YourServerRootDir/RustDedicated_Data/Managed` and make an empty file called `oxide.disable-sandbox`
  3. Add `// Reference: 0Harmony` on top of each of your plugins that using Harmony

  ```cs
// Reference: 0Harmony
using System;
using System.Collections.Generic;
using System.Linq;
  ```
> Harmony plugins now should be looking like this
  
--  
  Licensed under MIT. Comes without support & warranties.