This plugin depends on the [Robots](https://github.com/visose/Robots) plugin. There is a known problem when loading Grasshopper assemblies that depend on other 3rd party assemblies. Therefore, when installing, we have two options:

**1. Use the Components folder**

Install both the Robots plugin and the HANDZONe plugin in the Components folder. Then rename the Handzone.gha file to ZHandzone.gha to ensure it's loaded at the end. Ensure the file is unblocked in properties.

**2. Use drag and drop**

Unblock the Handzone.gha file in properties and then drag and drop it into the Grasshopper window after the Robots plugin is already loaded. This will load it for one time use only.
