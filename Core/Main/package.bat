mkdir OutputPackaged

REM Merging OsmSharp.Routing.dll
ilmerge /target:windll /targetplatform:v4 /out:OutputPackaged/OsmSharp.Routing.dll Output/OsmSharp.Routing.Core.dll Output/OsmSharp.Routing.Core.TSP.dll Output/OsmSharp.Routing.Core.VRP.dll Output/OsmSharp.Routing.CH.dll

REM Merging OsmSharp.Osm.dll
ilmerge /target:windll /targetplatform:v4 /out:OutputPackaged/OsmSharp.Osm.dll Output/OsmSharp.Osm.Core.dll Output/OsmSharp.Osm.Data.Core.dll Output/OsmSharp.Osm.Data.Oracle.dll Output/OsmSharp.Osm.Data.PBF.dll Output/OsmSharp.Osm.Data.PostgreSQL.dll Output/OsmSharp.Osm.Data.Redis.dll Output/OsmSharp.Osm.Data.SQLite.dll Output/OsmSharp.Osm.Data.XML.dll Output/OsmSharp.Osm.Routing.dll

REM Merging OsmSHarp.Tools.dll
ilmerge /target:windll /targetplatform:v4 /out:OutputPackaged/OsmSharp.Tools.dll Output/OsmSharp.Tools.Core.dll Output/OsmSharp.Tools.GeoCoding.dll Output/OsmSharp.Tools.GeoCoding.Custom.dll Output/OsmSharp.Tools.GeoCoding.Nomatim.dll Output/OsmSharp.Tools.Math.dll Output/OsmSharp.Tools.Math.AI.Genetic.dll Output/OsmSharp.Tools.Math.TSP.dll Output/OsmSharp.Tools.Math.VRP.Core.dll Output/OsmSharp.Tools.Math.VRP.MultiSalesman.dll Output/OsmSharp.Tools.TSPLIB.dll Output/OsmSharp.Tools.Xml.dll Output/OsmSharp.Tools.Xml.Gpx.dll Output/OsmSharp.Tools.Xml.Kml.dll Output/OsmSharp.Tools.Xml.Nomatim.dll

REM Merging OsmSharp.Winforms.dll
ilmerge /target:windll /targetplatform:v4 /out:OutputPackaged/OsmSharp.Winforms.dll Output/OsmSharp.Osm.Interpreter.dll Output/OsmSharp.Osm.Map.dll Output/OsmSharp.Osm.Renderer.Gdi.dll Output/OsmSharp.Osm.UI.Model.dll Output/OsmSharp.Osm.UI.WinForms.dll