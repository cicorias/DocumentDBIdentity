

- Disable compile on /Startup & /Model
- Fix namespaces
- Add Reference to DocumentDB dependancy clinet library


- IdentityModelsDocDb.cs
  - add using: using DX.TED.DocumentDb.Identity;
  IdentityConfigDocDb.cs
	- //TODO: these need to be nuget transforms
		using DX.TED.DocumentDb.Identity.Models;
		using DX.TED.DocumentDb.Identity;
