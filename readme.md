OsmSharp
========

OsmSharp is an open-source mapping tool designed to work with OpenStreetMap. Most important features are offline rendering of vector-data and routing. All OsmSharp features are available on Android, iOS, Windows Phone (using the Xamarin products) and the regulars Linux, Windows, OSX (using Mono).

Build status:

<img src="http://osmsharp.com:8080/app/rest/builds/buildType:(id:bt16)/statusIcon"/>

Features
--------

### Vector Rendering
Rendering OpenStreetMap-data using MapCSS or a custom style interpreter. Using a vector format for mobile devices rendering offline map data on Android/iOS is also possible.

<p>
	<img src="http://osmsharp.com/sites/default/files/iphone_screenshot1.png" width="300"/><img src="http://osmsharp.com/sites/default/files/iphone_screenshot2.png" width="300"/>
	<img src="http://osmsharp.com/sites/default/files/mapscreenshot3.png" width="600"/>
</p>

### Routing
Routing also using OpenStreetMap-data. Custom routing profiles are possible and offline routing on mobile devices can be done using a pre-processed binary format.

<p>
	<img src="http://osmsharp.com/sites/default/files/error_correction_dykstra.png"/>
</p>

### Optimisation Code
OsmSharp started as a project for logistical optimisation. There are several solvers for the (A)TSP and some VRP's.

<p>
	<img src="http://osmsharp.com/sites/default/files/optimization.png" width="600"/>
</p>

### Data Processing
OpenStreetMap data can be hard to handle sometimes. Basic filtering, conversion and database providers exists to import/export OpenStreetMap data.